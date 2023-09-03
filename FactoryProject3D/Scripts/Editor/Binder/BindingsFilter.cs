using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BindingsFilter : Editor
{
    private const string bindingsFilename = "Bindings.json";
    private const string drainageKeyName = "ДР";
    private const string drainageParam = "#Вентиль";

    [MenuItem("T-Soft/Binding/FilterDrainageInBindings")]
    public static void FilterDrainage()
    {
        var bindings = new List<TrenObjectData>();

        if (Utilities.CheckStreamingAssetsPath())
        {
            if (File.Exists(Application.streamingAssetsPath + '\\' + bindingsFilename))
            {
                using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + '\\' + bindingsFilename))
                {
                    try
                    {
                        bindings = JsonUtility.FromJson<TrenObjectDataList>(reader.ReadToEnd()).Data;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }

        int count = 0;
        if (bindings != null && bindings.Count > 0)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                if (bindings[i].TrenName.Contains(drainageKeyName))
                {
                    var newBind = bindings[i];
                    newBind.TrenParameter = drainageParam;
                    bindings[i] = newBind;
                    count++;
                }
            }
        }

        BindingManager.SaveTrenObjectsData(bindings);

        Debug.Log($"Drainage filtered: {count}.");
    }
}
