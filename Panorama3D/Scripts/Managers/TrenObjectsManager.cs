using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TrenObjectsManager
{
    private const string templateTypesName = "TemplateTypes";

    public static Dictionary<uint, RegistratedTrenObjectInfo> RegistratedTrenObjects { get; private set; }
    public static bool isConnected = false;

    public static void ConnectToSimulator(string ip)
    {
        int messages = ClientSocketManager.CreateConnect(ip, 20200, null);
        if (messages == 0)
        {
            isConnected = true;
        }
        else
        {
            isConnected = false;
            Debug.LogError($"Connection to Simulator Error: {messages}");
        }
    }

    public static void RegistrateObjects(Bindings bindings)
    {
        if (isConnected && bindings.Data != null)
        {
            RegistratedTrenObjects = new Dictionary<uint, RegistratedTrenObjectInfo>();
            var templates = Resources.Load<TemplateTypes>(templateTypesName);
            if (templates != null)
            {
                foreach (var binding in bindings.Data)
                {
                    var template = templates.GetTemplate(binding.TemplateName);
                    if (template)
                    {
                        var controllers = template.GetComponentsInChildren<TrenObjectControllerBase>();
                        foreach (var controller in controllers)
                        {
                            if (!RegistratedTrenObjects.Any(h => h.Value.TrenObjectName == binding.TrenObjectName &&
                                                            h.Value.TrenObjectParameter == controller.ParameterName))
                            {
                                var newInfo = new RegistratedTrenObjectInfo();
                                newInfo.TrenObjectName = binding.TrenObjectName;
                                newInfo.TrenObjectParameter = controller.ParameterName;

                                unsafe
                                {
                                    fixed (float* getValue = &newInfo.pointValueGet, setValue = &newInfo.pointValueSet)
                                    {
                                        uint index = ClientSocketManager.RegistrateParameter(binding.TrenObjectName,
                                                                                             controller.ParameterName,
                                                                                             controller.DataType,
                                                                                             getValue,
                                                                                             controller.DataType,
                                                                                             setValue);

                                        if (index != uint.MaxValue && !RegistratedTrenObjects.ContainsKey(index))
                                        {
                                            RegistratedTrenObjects.Add(index, newInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

public class RegistratedTrenObjectInfo
{
    public string TrenObjectName;
    public string TrenObjectParameter;
    public float pointValueGet;
    public float pointValueSet;
}