using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerItemController : MonoBehaviour
{
    private const string unknowablePrimitiveName = "Неизвестный тип элемента";
    private const string primitveNamePrefix = "Опознано как ";
    private const string emptyPrimitve = "Пустой";
    private const string unityNamePattern = "Имя Unity: {0}";

    [SerializeField]
    private TextMeshProUGUI PrimitiveName;

    [SerializeField]
    private TMP_InputField TrenObjectNameRead;

    [SerializeField]
    private TMP_InputField TrenObjectParamNameRead;

    [SerializeField]
    private TMP_InputField TrenObjectNameWrite;

    [SerializeField]
    private TMP_InputField TrenObjectParamNameWrite;

    [SerializeField]
    private Toggle NamesToggle;

    [SerializeField]
    private Toggle ParamsToggle;

    [SerializeField]
    private TMP_Dropdown PrimitivesList;

    [SerializeField]
    private TextMeshProUGUI UnityNameHeader;

    public bool syncNames { get; set; }
    public bool syncParams { get; set; }

    private bool isPrimitivesListObjectInitialized = false;

    private string unityObjectName;

    private static List<string> primitivesNamesList;

    private void Awake()
    {
        if (NamesToggle)
        {
            syncNames = NamesToggle.isOn;
        }

        if (ParamsToggle)
        {
            syncParams = ParamsToggle.isOn;
        }

        if (primitivesNamesList == null)
        {
            InitializePrimitivesList();
        }

        if (!isPrimitivesListObjectInitialized)
        {
            InitializePrimitiveListObject();
        }
    }

    private void InitializePrimitivesList()
    {
        if (primitivesNamesList == null || primitivesNamesList.Count == 0)
        {
            primitivesNamesList = BindingManager.Primitives.Select(h => h.Name).ToList();
            primitivesNamesList.Add(emptyPrimitve);
        }
    }
    private void InitializePrimitiveListObject()
    {
        if (PrimitivesList)
        {
            PrimitivesList.ClearOptions();

            if (BindingManager.Primitives != null && BindingManager.Primitives.Length > 0)
            {
                PrimitivesList.AddOptions(primitivesNamesList);
            }

            isPrimitivesListObjectInitialized = true;
        }
    }

    public void Initialize(string unityName, PrimitiveObject primitive = null, 
                           TrenObjectData dataRead = default(TrenObjectData), 
                           TrenObjectData dataWrite = default(TrenObjectData))
    {
        if (primitivesNamesList == null)
        {
            InitializePrimitivesList();
        }

        if (!isPrimitivesListObjectInitialized)
        {
            InitializePrimitiveListObject();
        }

        if (PrimitiveName)
        {
            if (primitive)
            {
                PrimitiveName.text = primitveNamePrefix + primitive.Name;
            }
            else
            {
                PrimitiveName.text = unknowablePrimitiveName;
            }
        }

        if (dataRead == default(TrenObjectData) || dataWrite == default(TrenObjectData))
        {
            if (primitive)
            {
                if (TrenObjectParamNameRead)
                {
                    TrenObjectParamNameRead.SetTextWithoutNotify(primitive.GetParameterName);
                }

                if (TrenObjectParamNameWrite)
                {
                    TrenObjectParamNameWrite.SetTextWithoutNotify(primitive.SetParameterName);
                }
            }
        }
     
        if (dataRead != default(TrenObjectData))
        {
            if (TrenObjectNameRead)
            {
                TrenObjectNameRead.SetTextWithoutNotify(dataRead.TrenName);
            }

            if (TrenObjectParamNameRead)
            {
                TrenObjectParamNameRead.SetTextWithoutNotify(dataRead.TrenParameter);
            }
        }

        if (dataWrite != default(TrenObjectData))
        {
            if (TrenObjectNameWrite)
            {
                TrenObjectNameWrite.SetTextWithoutNotify(dataWrite.TrenName);
            }

            if (TrenObjectParamNameWrite)
            {
                TrenObjectParamNameWrite.SetTextWithoutNotify(dataWrite.TrenParameter);
            }
        }
        
        if (PrimitivesList && primitivesNamesList != null && primitivesNamesList.Count > 0)
        {
            if (primitive)
            {
                PrimitivesList.SetValueWithoutNotify(primitivesNamesList.IndexOf(primitive.Name));
            }
            else
            {
                PrimitivesList.SetValueWithoutNotify(primitivesNamesList.IndexOf(emptyPrimitve));
            }
        }

        unityObjectName = unityName;
        if (UnityNameHeader)
        {
            UnityNameHeader.text = string.Format(unityNamePattern, unityObjectName);
        }
    }

    public string GetInputFromField(TMP_InputField input)
    {
        if (input)
        {
            return input.text;
        }

        return null;
    }
    public string GetTrenObjectNameRead()
    {
        return GetInputFromField(TrenObjectNameRead);
    }
    public string GetTrenObjectParamNameRead()
    {
        return GetInputFromField(TrenObjectParamNameRead);
    }
    public string GetTrenObjectNameWrite()
    {
        return GetInputFromField(TrenObjectNameWrite);
    }
    public string GetTrenObjectParamNameWrite()
    {
        return GetInputFromField(TrenObjectParamNameWrite);
    }

    public TrenObjectData GetReadableTrenObjectData()
    {
        if (TrenObjectNameRead && TrenObjectParamNameRead)
        {
            return new TrenObjectData()
            {
                UnityName = unityObjectName,
                TrenName = TrenObjectNameRead.text,
                TrenParameter = TrenObjectParamNameRead.text,
                ObjectMode = ObjectMode.GET
            };
        }

        return default(TrenObjectData);
    }
    public TrenObjectData GetWritableTrenObjectData()
    {
        if (TrenObjectNameWrite && TrenObjectParamNameWrite)
        {
            return new TrenObjectData()
            {
                UnityName = unityObjectName,
                TrenName = TrenObjectNameWrite.text,
                TrenParameter = TrenObjectParamNameWrite.text,
                ObjectMode = ObjectMode.SET
            };
        }

        return default(TrenObjectData);
    }
    public void GetResultTrenObjectData(out TrenObjectData[] trenObjectDatas)
    {
        var read = GetReadableTrenObjectData();
        var write = GetWritableTrenObjectData();

        if (read.TrenName      == write.TrenName    &&
            read.UnityName     == write.UnityName   &&
            read.TrenParameter == write.TrenParameter)
        {
            trenObjectDatas = new TrenObjectData[1];
            trenObjectDatas[0] = read;
            trenObjectDatas[0].ObjectMode = ObjectMode.SETGET;
        }
        else
        {
            trenObjectDatas = new TrenObjectData[2];
            trenObjectDatas[0] = read;
            trenObjectDatas[1] = write;
        }
    }

    public void OnNamesToggleChanged(bool value)
    {
        if (value)
        {
            SetReadNameInWriteName();
        }
    }
    public void OnParamsToggleChanged(bool value)
    {
        if (value)
        {
            SetReadParamInWriteParam();
        }
    }

    public void OnReadNameChange()
    {
        if (syncNames)
        {
            SetReadNameInWriteName();
        }
    }
    public void OnWriteNameChange()
    {
        if (syncNames)
        {
            SetWriteNameInReadName();
        }
    }
    public void OnReadParamChange()
    {
        if (syncParams)
        {
            SetReadParamInWriteParam();
        }
    }
    public void OnWriteParamChange()
    {
        if (syncParams)
        {
            SetWriteParamInReadParam();
        }
    }

    private void SetReadNameInWriteName()
    {
        if (TrenObjectNameRead && TrenObjectNameWrite)
        {
            TrenObjectNameWrite.text = TrenObjectNameRead.text;
        }
    }
    private void SetWriteNameInReadName()
    {
        if (TrenObjectNameRead && TrenObjectNameWrite)
        {
            TrenObjectNameRead.text = TrenObjectNameWrite.text;
        }
    }
    private void SetReadParamInWriteParam()
    {
        if (TrenObjectParamNameRead && TrenObjectParamNameWrite)
        {
            TrenObjectParamNameWrite.text = TrenObjectParamNameRead.text;
        }
    }
    private void SetWriteParamInReadParam()
    {
        if (TrenObjectParamNameRead && TrenObjectParamNameWrite)
        {
            TrenObjectParamNameRead.text = TrenObjectParamNameWrite.text;
        }
    }
    public void SetReadWriteTrenName(string value)
    {
        if (TrenObjectNameRead)
        {
            TrenObjectNameRead.text = value;
        }

        if (TrenObjectNameWrite)
        {
            TrenObjectNameWrite.text = value;
        }
    }

    public void ClearData()
    {
        if (TrenObjectNameRead)
        {
            TrenObjectNameRead.text = string.Empty;
        }

        if (TrenObjectParamNameRead)
        {
            TrenObjectParamNameRead.text = string.Empty;
        }

        if (TrenObjectNameWrite)
        {
            TrenObjectNameWrite.text = string.Empty;
        }

        if (TrenObjectParamNameWrite)
        {
            TrenObjectParamNameWrite.text = string.Empty;
        }
    }
    public void SaveData()
    {
        GetResultTrenObjectData(out var data);
        BindingManager.AddTrenObject(data[0]);
        if (data.Length > 1)
        {
            BindingManager.AddTrenObject(data[1]);
        }
    }

    public void OnPrimitivesDropdownValueChanged(int value)
    {
        if (primitivesNamesList != null && primitivesNamesList.Count > value)
        {
            var primitiveName = primitivesNamesList[value];
            if (primitiveName != emptyPrimitve && BindingManager.Primitives != null && BindingManager.Primitives.Length > 0)
            {
                var primitive = BindingManager.Primitives.FirstOrDefault(h => h.Name == primitiveName);
                if (primitive != null)
                {
                    if (TrenObjectParamNameRead)
                    {
                        TrenObjectParamNameRead.text = primitive.GetParameterName;
                    }

                    if (TrenObjectParamNameWrite)
                    {
                        TrenObjectParamNameWrite.text = primitive.SetParameterName;
                    }
                }
                else
                {
                    Debug.LogError($"Primitive with name {primitiveName} was not found.");
                }
            }
            else
            {
                if (TrenObjectParamNameRead)
                {
                    TrenObjectParamNameRead.text = string.Empty;
                }

                if (TrenObjectParamNameWrite)
                {
                    TrenObjectParamNameWrite.text = string.Empty;
                }
            }
        }
    }
}
