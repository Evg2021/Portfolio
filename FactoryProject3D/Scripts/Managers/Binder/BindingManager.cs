using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BindingManager : SingletonMonoBehaviour<BindingManager>
{
    private const string primitivesPath = "MainPrimitives";
    private const string bindingsFilename = "Bindings.json";
    private const string networkManagerName = "NetworkManager";
    private const string canvasName = "Canvas";

    public static Transform CurrentController { get; private set; }
    public static InteractableObject CurrentInteractableObject { get; private set; }

    public static PrimitiveObject[] Primitives { get; private set; }
    public static List<TrenObjectData> Bindings { get; private set; }

    [SerializeField]
    private BindingPanelController bindingPanelPrefab;
    private BindingPanelController bindingPanel;

    [SerializeField]
    private BindingUIController bindingUIControllerPrefab;
    private BindingUIController bindingUIController;

    [SerializeField]
    private MarkerController bindingMarker;
    private List<MarkerController> markers;

    public List<TrenObjectData> NonbindedObjects;

    private Transform binder;

    public static bool IsBindingsShown { get; private set; }
    public static bool IsBindingPanelOpened { get; private set; }
    public static bool isConnectedToSim { get; private set; }
    public static int InteractableObjectsCount { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        InitializePrimitives();
        InitializeBindings();
        InitializeBindingPanel();
        InitializeInteractableObjects();
        InitializeInteractableObjectsCount();
        InitializeBindingUI();

        DisableClient();

        Cursor.lockState = CursorLockMode.Locked;
        IsBindingsShown = false;
        isConnectedToSim = false;
    }

    private void InitializeInteractableObjects()
    {
        var interactables = FindObjectsOfType<InteractableObject>();
        foreach (var interactable in interactables)
        {
            interactable.InitializeRenderers(true);
        }
    }
    private void InitializeBindingUI()
    {
        if (bindingUIControllerPrefab)
        {
            var canvas = GameObject.Find(canvasName);
            if (canvas)
            {
                bindingUIController = Instantiate(bindingUIControllerPrefab, canvas.transform);
                if (Bindings != null)
                {
                    bindingUIController.SetBindingsCount(Bindings.Count);
                }

                bindingUIController.SetObjectsCount(InteractableObjectsCount);
            }
        }
    }
    private void InitializeBindingPanel()
    {
        if (bindingPanelPrefab)
        {
            var canvas = GameObject.Find(canvasName);
            if (canvas)
            {
                bindingPanel = Instantiate(bindingPanelPrefab, canvas.transform);
            }
        }
    }
    private void InitializePrimitives()
    {
        var primitivesObjects = Resources.LoadAll(primitivesPath, typeof(PrimitiveObject));
        Primitives = new PrimitiveObject[primitivesObjects.Length];
        for (int i = 0; i < primitivesObjects.Length; i++)
        {
            Primitives[i] = (PrimitiveObject)primitivesObjects[i];
        }
    }
    private void InitializeBindings()
    {
        Bindings = new List<TrenObjectData>();

        if (Utilities.CheckStreamingAssetsPath())
        {
            if (File.Exists(Application.streamingAssetsPath + '\\' + bindingsFilename))
            {
                using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + '\\' + bindingsFilename))
                {
                    try
                    {
                        Bindings = JsonUtility.FromJson<TrenObjectDataList>(reader.ReadToEnd()).Data;
                        FilterBindings(Bindings, out var newBindings);
                        if (newBindings != null && newBindings.Count > 0)
                        {
                            Bindings = newBindings;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }
    }
    private void InitializeInteractableObjectsCount()
    {
        int manipulatorsCount = FindObjectsOfType<Transform>().Where(h => h.name.Contains("Manipulator")).Count();
        int flapsWithoutValvesCount = FindObjectsOfType<ValuesShowingController>().Where(h => h.name.Contains("Flap")).Count();
        int handlesCount = FindObjectsOfType<Transform>().Where(h => h.name.Contains("Handle") || h.name.Contains("handle")).Count();

        InteractableObjectsCount = manipulatorsCount + flapsWithoutValvesCount + handlesCount;
    }

    private void FilterBindings(List<TrenObjectData> trenObjects, out List<TrenObjectData> result)
    {
        result = null;
        if (trenObjects != null && trenObjects.Count > 0)
        {
            result = new List<TrenObjectData>();
            foreach (var trenObject in trenObjects)
            {
                var sameObjects = result.Where(h => h.UnityName == trenObject.UnityName);

                if (sameObjects == null || sameObjects.Count() == 0)
                {
                    result.Add(trenObject);
                }
                else
                {
                    if (!sameObjects.Any(h => h.ObjectMode == ObjectMode.SETGET))
                    {
                        if ((trenObject.ObjectMode == ObjectMode.GET &&
                             !sameObjects.Any(h => h.ObjectMode == ObjectMode.GET)) ||
                            (trenObject.ObjectMode == ObjectMode.SET &&
                             !sameObjects.Any(h => h.ObjectMode == ObjectMode.SET)))
                        {
                            result.Add(trenObject);
                        }
                    }
                }
            }
        }
    }

    public static void SetCurrentController(Transform controller)
    {
        CurrentController = controller;
        CurrentInteractableObject = null;

        if (Instance)
        {
            Instance.ShowBindingPanel();
        }
    }
    public static void SetCurrentInteractableObject(InteractableObject interactable)
    {
        CurrentInteractableObject = interactable;
        CurrentController = null;

        if (Instance)
        {
            Instance.ShowBindingPanel();
        }
    }

    private void ShowBindingPanel()
    {
        if (Binder.Instance)
        {
            Binder.Instance.DisableController();
        }

        if (bindingPanel)
        {
            if (CurrentController != null)
            {
                PrimitiveObject primitive = GetPrimitiveType(CurrentController);

                if (CheckExistedBinding(CurrentController.name, out var data))
                {
                    bindingPanel.ShowPrimitiveSettings(CurrentController, primitive, data);
                }
                else
                {
                    bindingPanel.ShowPrimitiveSettings(CurrentController, primitive);
                }
            }
            else
            {
                bindingPanel.ShowComplexObjectSettings(CurrentInteractableObject);
            }
        }

        Cursor.lockState = CursorLockMode.None;

        IsBindingPanelOpened = true;
    }
    public void HideBindingPanel()
    {
        if (Binder.Instance)
        {
            Binder.Instance.EnableController();
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (bindingPanel)
        {
            bindingPanel.HidePanel();
        }

        IsBindingPanelOpened = false;
    }

    public static void SaveTrenObjectsData(List<TrenObjectData> trenObjects)
    {
        if (trenObjects != null && trenObjects.Count > 0)
        {
            Utilities.CheckStreamingAssetsPath();

            using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + '\\' + bindingsFilename, false))
            {
                var data = new TrenObjectDataList()
                {
                    Data = trenObjects
                };
                writer.Write(JsonUtility.ToJson(data, true));
            }
        }
    }
    public static void SaveCurrentBindings()
    {
        SaveTrenObjectsData(Bindings);
    }

    public static void AddTrenObject(TrenObjectData data)
    {
        if (string.IsNullOrEmpty(data.TrenName) || string.IsNullOrEmpty(data.TrenParameter) || string.IsNullOrEmpty(data.UnityName))
        {
            return;
        }

        if (Bindings != null)
        {
            if (Bindings.Count == 0)
            {
                Bindings.Add(data);
            }
            else
            {
                if (!Bindings.Contains(data))
                {
                    if (data.ObjectMode != ObjectMode.SETGET)
                    {
                        var identityBinding = Bindings.FirstOrDefault(h => h.UnityName == data.UnityName &&
                                                                           h.ObjectMode == data.ObjectMode);

                        var identityGetSetBinding = Bindings.FirstOrDefault(h => h.UnityName == data.UnityName && 
                                                                                 h.ObjectMode == ObjectMode.SETGET);
                        
                        if (identityGetSetBinding != default(TrenObjectData))
                        {
                            Bindings.Remove(identityGetSetBinding);
                            var trenObjectInsideGetSet = new TrenObjectData()
                            {
                                UnityName = identityGetSetBinding.UnityName,
                                TrenName = identityGetSetBinding.TrenName,
                                TrenParameter = identityGetSetBinding.TrenParameter
                            };

                            trenObjectInsideGetSet.ObjectMode = data.ObjectMode == ObjectMode.GET ? ObjectMode.SET : ObjectMode.GET;

                            Bindings.Add(trenObjectInsideGetSet);
                        }

                        if (identityBinding != default(TrenObjectData))
                        {
                            Bindings.Remove(identityBinding);
                            Bindings.Add(data);
                        }
                        else
                        {
                            Bindings.Add(data);
                        }
                    }
                    else
                    {
                        var identityBindingGet = Bindings.FirstOrDefault(h => h.UnityName     == data.UnityName     &&
                                                                              //h.TrenName      == data.TrenName      &&
                                                                              //h.TrenParameter == data.TrenParameter &&
                                                                              h.ObjectMode    == ObjectMode.GET     );  
                             

                        var identityBindingSet = Bindings.FirstOrDefault(h => h.UnityName     == data.UnityName     &&
                                                                              //h.TrenName      == data.TrenName      &&
                                                                              //h.TrenParameter == data.TrenParameter &&
                                                                              h.ObjectMode    == ObjectMode.SET     );

                        var identityBindingGetSet = Bindings.FirstOrDefault(h => h.UnityName  == data.UnityName     &&
                                                                              //h.TrenName      == data.TrenName      &&
                                                                              //h.TrenParameter == data.TrenParameter &&
                                                                              h.ObjectMode    == ObjectMode.SETGET  );

                        if (identityBindingGet != default(TrenObjectData))
                        {
                            Bindings.Remove(identityBindingGet);
                        }
                        if (identityBindingSet != default(TrenObjectData))
                        {
                            Bindings.Remove(identityBindingSet);
                        }
                        if (identityBindingGetSet != default(TrenObjectData))
                        {
                            Bindings.Remove(identityBindingGetSet);
                        }

                        Bindings.Add(data);
                    }
                }
            }
        }
        else
        {
            Bindings = new List<TrenObjectData>();
            Bindings.Add(data);
        }

        if (Instance.bindingUIController)
        {
            Instance.bindingUIController.SetBindingsCount(Bindings.Count);
        }

        if (IsBindingsShown && Instance && Instance.bindingMarker)
        {
            if (Instance.markers != null)
            {
                var point = GameObject.Find(data.UnityName);
                if (point)
                {
                    var newMarker = Instantiate(Instance.bindingMarker, point.transform.position, Quaternion.identity);
                    newMarker.Initialize(data.TrenName, data.TrenParameter);
                    if (isConnectedToSim)
                    {
                        newMarker.Test();
                    }

                    Instance.markers.Add(newMarker);
                }
            }
        }
    }

    public static PrimitiveObject GetPrimitiveType(Transform transform)
    {
        if (Primitives != null && Primitives.Length > 0)
        {
            foreach (var primitive in Primitives)
            {
                if(string.IsNullOrEmpty(primitive.KeyName) && string.IsNullOrEmpty(primitive.ParentKeyName))
                {
                    continue;
                }

                if (!transform.parent)
                {
                    if (transform.name.Contains(primitive.KeyName))
                    {
                        return primitive;
                    }
                }
                else
                {
                    if (transform.parent.parent)
                    {
                        if (transform.name.Contains(primitive.KeyName) && transform.parent.parent.name.Contains(primitive.ParentKeyName))
                        {
                            return primitive;
                        }
                    }                        

                    if (transform.name.Contains(primitive.KeyName) && transform.parent.name.Contains(primitive.ParentKeyName))
                    {
                        return primitive;
                    }

                }
            }
        }

        return null;
    }
    public static bool CheckExistedBinding(string unityName, out TrenObjectData[] data)
    {
        data = null;

        if (Bindings != null && Bindings.Count > 0)
        {
            data = Bindings.Where(h => h.UnityName == unityName).ToArray();
            if (data.Length > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void DisableClient()
    {
        var camera = Camera.main;
        if (camera && camera.gameObject)
        {
            camera.gameObject.SetActive(false);
        }

        var networkManager = GameObject.Find(networkManagerName);
        if (networkManager)
        {
            networkManager.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        SaveTrenObjectsData(Bindings);
    }

    private static List<Transform> GetBindedObjects()
    {
        var result = new List<Transform>();
        if (Bindings != null && Bindings.Count > 0)
        {
            foreach (var bind in Bindings)
            {
                if (!string.IsNullOrEmpty(bind.UnityName))
                {
                    var bindedObject = GameObject.Find(bind.UnityName);
                    if (bindedObject)
                    {
                        result.Add(bindedObject.transform);
                    }
                }

            }
        }

        return result;
    }

    public static void ShowBindedObjects()
    {
        if (Instance && Instance.bindingMarker && !IsBindingsShown)
        {
            if (Instance.markers == null)
            {
                Instance.markers = new List<MarkerController>();
            }

            var bindings = GetBindedObjects();
            foreach (var bind in bindings)
            {
                var bindData = Bindings.FirstOrDefault(h => h.UnityName == bind.name);
                Debug.Log(bindData.UnityName);
                if (bindData != default(TrenObjectData))
                {
                    var marker = Instantiate(Instance.bindingMarker, bind.position, Quaternion.identity);
                    marker.Initialize(bindData.TrenName, bindData.TrenParameter);
                    Instance.markers.Add(marker);
                }
            }

            IsBindingsShown = true;
        }
    }
    public static void HideBindedObjects()
    {
        if (Instance && IsBindingsShown)
        {
            if (Instance.markers != null && Instance.markers.Count > 0)
            {
                foreach (var marker in Instance.markers)
                {
                    Destroy(marker.gameObject);
                }

                Instance.markers.Clear();
            }

            IsBindingsShown = false;
        }
    }

    public static bool ConnectToSim()
    {
        if (Instance)
        {
            if (Instance.markers.Count > 0)
            {
                var message = ClientSocketManager.CreateConnect("127.0.0.1", 20200, null);
                if (message == 0)
                {
                    foreach (var marker in Instance.markers)
                    {
                        marker.Test();
                    }
                    isConnectedToSim = true;
                    return true;
                }
            }
        }
        isConnectedToSim = false;
        return false;
    }
    public static void DisconnectFromSim()
    {
        ClientSocketManager.ClearRegistration();
        ClientSocketManager.CloseConnect();
        isConnectedToSim = false;

        if (Instance)
        {
            if (Instance.markers != null && Instance.markers.Count > 0)
            {
                foreach (var marker in Instance.markers)
                {
                    marker.DisableTest();
                }
            }
        }
    }

    private void OnDisable()
    {
        DisconnectFromSim();
    }

    private void OnGUI()
    {
        if (!binder && transform.childCount > 0)
        {
            binder = transform.GetChild(0);
        }

        if (binder)
        {
            GUI.Box(new Rect(35, 10, 140, 25), $"X:{Mathf.Round(binder.position.x * 10) / 10}, " +
                                                                  $"Y:{Mathf.Round(binder.position.y * 10) / 10}, " +
                                                                  $"Z:{Mathf.Round(binder.position.z * 10) / 10}");
        }
    }
}
