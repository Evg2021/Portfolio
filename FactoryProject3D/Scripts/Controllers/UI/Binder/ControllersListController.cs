using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ControllersListController : MonoBehaviour
{
    [SerializeField]
    private ControllerItemController ControllerItemPrefab;

    [SerializeField]
    private Transform Content;

    [SerializeField]
    private TMP_InputField mainTrenObjectName;

    private List<ControllerItemController> controllers;
    private bool isInitialized = false;

    public void ClearList()
    {
        if (controllers != null)
        {
            foreach (var controller in controllers)
            {
                Destroy(controller.gameObject);
            }

            controllers.Clear();
        }

        isInitialized = false;
    }

    public void Initialize(string[] unityNames, PrimitiveObject[] primitives = null)
    {
        string mainName = string.Empty;
        int matchedNamesCount = 0;
        for (int i = 0; i < unityNames.Length; i++)
        {
            if (!string.IsNullOrEmpty(unityNames[i]))
            {
                PrimitiveObject primitive = null;
                TrenObjectData dataRead = default(TrenObjectData);
                TrenObjectData dataWrite = default(TrenObjectData);

                if (primitives.Length > i)
                {
                    primitive = primitives[i];
                }

                if (BindingManager.CheckExistedBinding(unityNames[i], out TrenObjectData[] data))
                {
                    if (string.IsNullOrEmpty(mainName))
                    {
                        mainName = data[0].TrenName;
                    }

                    if (data.Length == 1 && data[0].ObjectMode == ObjectMode.SETGET)
                    {
                        dataRead = data[0];
                        dataRead.ObjectMode = ObjectMode.GET;

                        dataWrite = data[0];
                        dataWrite.ObjectMode = ObjectMode.SET;
                    }
                    else
                    {
                        dataRead = data.FirstOrDefault(h => h.ObjectMode == ObjectMode.GET);
                        dataWrite = data.FirstOrDefault(h => h.ObjectMode == ObjectMode.SET);
                    }

                    if (mainName == data[0].TrenName)
                    {
                        if ((data.Length == 2 && mainName == data[1].TrenName) || data.Length == 1)
                        {
                            matchedNamesCount++;
                        }
                    }
                }

                CreateControllerItem(unityNames[i], primitive, dataRead, dataWrite);
            }
        }

        if (mainTrenObjectName)
        {
            mainTrenObjectName.SetTextWithoutNotify(mainName);
        }
    }

    public void CreateControllerItem(string unityName, PrimitiveObject primitive = null, 
                                     TrenObjectData dataRead = default(TrenObjectData), 
                                     TrenObjectData dataWrite = default(TrenObjectData))
    {
        if (Content && ControllerItemPrefab)
        {
            var item = Instantiate(ControllerItemPrefab, Content);
            item.Initialize(unityName, primitive, dataRead, dataWrite);

            if (controllers == null)
            {
                controllers = new List<ControllerItemController>();
            }

            controllers.Add(item);
        }

        isInitialized = true;
    }

    public void SaveData()
    {
        if (controllers != null && controllers.Count > 0)
        {
            foreach (var controller in controllers)
            {
                controller.SaveData();
            }
        }
    }

    public void OnChangeMainTrenObjectName(string value)
    {
        if (controllers != null && controllers.Count > 0)
        {
            foreach (var controller in controllers)
            {
                controller.SetReadWriteTrenName(value);
            }
        }
    }
}
