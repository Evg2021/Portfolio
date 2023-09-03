using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableObjectWithPult : InteractableObject
{
    private const string canvasName = "Canvas";
    private const float simulatorUpdateInterval = 1.0f;

    private Dictionary<string, TrenObjectData> localBindings;

    public bool IsPultShown { get; private set; }

    private static Transform canvasTransform;

    public GameObject PultPrefab;
    private GameObject generatedPult;

    private TrenObject[] trenObjects;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (!canvasTransform)
        {
            canvasTransform = GameObject.Find(canvasName).transform;
        }
    }

    public override PrimitiveObject[] GetPrimitives()
    {
        ControllerBase[] inInteractableControllers = GetComponentsInChildren<ControllerBase>();
        ControllerBase[] pultControllers = null;
        if (PultPrefab)
        {
            pultControllers = PultPrefab.GetComponentsInChildren<ControllerBase>();
        }

        int length = 0;
        length += inInteractableControllers != null ? inInteractableControllers.Length : 0;
        length += pultControllers != null ? pultControllers.Length : 0;
        PrimitiveObject[] result = new PrimitiveObject[length];

        if (inInteractableControllers != null)
        {
            for (int i = 0; i < inInteractableControllers.Length; i++)
            {
                result[i] = BindingManager.GetPrimitiveType(inInteractableControllers[i].transform);
            }
        }

        if (pultControllers != null)
        {
            for (int i = 0; i < pultControllers.Length; i++)
            {
                result[inInteractableControllers.Length + i] = BindingManager.GetPrimitiveType(pultControllers[i].transform);
            }
        }

        return result;
    }

    public void AddBinding(TrenObjectData data)
    {
        if (PultPrefab)
        {
            if (localBindings == null) localBindings = new Dictionary<string, TrenObjectData>();

            var splittedName = data.UnityName.Split('\'');
            if (splittedName.Length > 1)
            {
                string controllerObjectName = splittedName[splittedName.Length - 1];
                var controllerObject = PultPrefab.GetComponentsInChildren<Transform>().FirstOrDefault(h => h.name == controllerObjectName);
                if (controllerObject)
                {
                    localBindings.Add(controllerObjectName, data);
                }
            }
        }
    }

    public void ShowPult()
    {
        if (canvasTransform && PultPrefab && localBindings != null && localBindings.Count > 0 && IsInitialized)
        {
            generatedPult = Instantiate(PultPrefab, canvasTransform);
            foreach (var binding in localBindings)
            {
                var controllerObject = generatedPult.GetComponentsInChildren<Transform>().FirstOrDefault(h => h.name == binding.Key);
                if (controllerObject && controllerObject.TryGetComponent<TrenObject>(out var trenObject))
                {
                    trenObject.Initialize(binding.Value.TrenName, binding.Value.TrenParameter, binding.Value.ObjectMode);
                    trenObject.RegistrateObject();
                    if (controllerObject.TryGetComponent<ControllerBase>(out var controller))
                    {
                        controller.Initialize();
                    }
                }
            }

            IsPultShown = true;

            trenObjects = generatedPult.GetComponentsInChildren<TrenObject>().Where(h => h.IsRegistrated).ToArray();
            currentRoutine = StartCoroutine(UpdateTrenObjects());
        }
    }
    public void HidePult()
    {
        if (generatedPult)
        {
            StopCurrentRoutine();

            var trenObjects = generatedPult.GetComponentsInChildren<TrenObject>();
            foreach (var trenObject in trenObjects)
            {
                ClientSocketManager.RemoveRegistratedParameter(trenObject.GetTrenIndex());
            }

            Destroy(generatedPult);

            IsPultShown = false;
        }
    }

    private IEnumerator UpdateTrenObjects()
    {
        while (IsPultShown && trenObjects != null && trenObjects.Length > 0)
        {
            for (int i = 0; i < trenObjects.Length; i++)
            {
                trenObjects[i].UpdateGetter();
            }

            yield return new WaitForSeconds(simulatorUpdateInterval);
        }

        currentRoutine = null;
    }
    private void StopCurrentRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }
}
