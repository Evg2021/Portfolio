using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BindingPanelController : MonoBehaviour
{
    [SerializeField]
    private ControllersListController controllersList;

    [SerializeField]
    private ControllerItemController primitiveSettings;

    [SerializeField]
    private GameObject complexObjectSettings;

    [SerializeField]
    private TextMeshProUGUI countIndentifiedTrenObjectsHeader;

    public bool isPrimitiveSettings { get; private set; }
    public bool isComplexObjectSettings { get; private set; }

    private string countIdentifiedTrenObjectsPattern;
    private int countIdentifiedTrenObjects;

    public void HidePanel()
    {
        SetActivePrimitiveSettings(false);
        SetActiveComplexObjectSettings(false);
        gameObject.SetActive(false);

        if (primitiveSettings)
        {
            primitiveSettings.ClearData();
        }

        if (controllersList)
        {
            controllersList.ClearList();
        }
    }
    public void ShowPrimitiveSettings(Transform controller, PrimitiveObject primitive, TrenObjectData[] data = null)
    {
        SetActiveComplexObjectSettings(false);
        SetActivePrimitiveSettings(true);

        if (primitiveSettings)
        {
            if (data != null)
            {
                if (data.Length == 1)
                {
                    if (data[0].ObjectMode == ObjectMode.SETGET)
                    {
                        primitiveSettings.Initialize(controller.name, primitive, data[0], data[0]);
                    }
                    else
                    {
                        if (data[0].ObjectMode == ObjectMode.GET)
                        {
                            primitiveSettings.Initialize(controller.name, primitive, data[0], default(TrenObjectData));
                        }
                        else
                        {
                            primitiveSettings.Initialize(controller.name, primitive, default(TrenObjectData), data[0]);
                        }
                    }
                }
                else if (data.Length == 2)
                {
                    var dataRead = data.FirstOrDefault(h => h.ObjectMode == ObjectMode.GET);
                    var dataWrite = data.FirstOrDefault(h => h.ObjectMode == ObjectMode.SET);
                    primitiveSettings.Initialize(controller.name, primitive, dataRead, dataWrite);
                }
            }
            else
            {
                primitiveSettings.Initialize(controller.name, primitive);
            }
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
    public void ShowComplexObjectSettings(InteractableObject interactable)
    {
        SetActivePrimitiveSettings(false);
        SetActiveComplexObjectSettings(true);

        if (controllersList)
        {
            var primitives = interactable.GetPrimitives();
            var names = interactable.GetComponentsInChildren<ControllerBase>().Select(h => h.name).ToArray();

            if (interactable is InteractableObjectWithPult)
            {
                var interactableWithPult = interactable as InteractableObjectWithPult;
                if (interactableWithPult.PultPrefab)
                {
                    var pultControllersNames = interactableWithPult.PultPrefab.GetComponentsInChildren<ControllerBase>().Select(h => h.name).ToArray();
                    var existedNames = names;
                    names = new string[pultControllersNames.Length + existedNames.Length];
                    for (int i = 0; i < existedNames.Length; i++)
                    {
                        names[i] = existedNames[i];
                    }
                    for (int i = 0; i < pultControllersNames.Length; i++)
                    {
                        names[i + existedNames.Length] = interactable.name + '\'' + pultControllersNames[i];
                    }
                }
            }

            controllersList.Initialize(names, primitives);

            if (countIndentifiedTrenObjectsHeader)
            {
                if (string.IsNullOrEmpty(countIdentifiedTrenObjectsPattern))
                {
                    countIdentifiedTrenObjectsPattern = countIndentifiedTrenObjectsHeader.text;
                }

                int count = primitives != null ? primitives.Where(h => h != null).Count() : 0;

                countIndentifiedTrenObjectsHeader.text = string.Format(countIdentifiedTrenObjectsPattern, count);
            }
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }


    private void SetActivePrimitiveSettings(bool value)
    {
        if (primitiveSettings && isPrimitiveSettings != value)
        {
            primitiveSettings.gameObject.SetActive(value);
            isPrimitiveSettings = value;
        }
    }
    private void SetActiveComplexObjectSettings(bool value)
    {
        if (complexObjectSettings && isComplexObjectSettings != value)
        {
            complexObjectSettings.SetActive(value);
            isComplexObjectSettings = value;
        }
    }

    public void OnClickCancelButton()
    {
        var manager = BindingManager.Instance;
        if (manager)
        {
            manager.HideBindingPanel();
        }
    }
}
