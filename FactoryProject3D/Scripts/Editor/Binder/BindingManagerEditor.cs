using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BindingManager))]
public class BindingManagerEditor : Editor
{
    private const string nonbindingsFilename = "Nonbindings.json";
    private List<TrenObjectData> nonbindedObjects = new List<TrenObjectData>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manager = (BindingManager)target;

        if (GUILayout.Button("Get nonbinded objects"))
        {
            var bindings = BindingManager.Bindings;
            var nonbindedMarkers = FindObjectsOfType<MarkerController>().Where(h => !h.registred);
            if (bindings != null && bindings.Count > 0 && nonbindedMarkers != null && nonbindedMarkers.Count() > 0)
            {
                nonbindedObjects = new List<TrenObjectData>();
                foreach (var nonbindedMarker in nonbindedMarkers)
                {
                    var bind = bindings.FirstOrDefault(h => h.TrenName == nonbindedMarker.objName && h.TrenParameter == nonbindedMarker.paramName);
                    if (!string.IsNullOrEmpty(bind.TrenName) && !string.IsNullOrEmpty(bind.TrenParameter))
                    {
                        nonbindedObjects.Add(bind);
                    }
                }
            }

            manager.NonbindedObjects = nonbindedObjects;
        }

        if (GUILayout.Button("Save nonbinded objects"))
        {
            if (nonbindedObjects != null && nonbindedObjects.Count > 0)
            {
                var data = new TrenObjectDataList()
                {
                    Data = nonbindedObjects
                };

                using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + '\\' + nonbindingsFilename))
                {
                    writer.Write(JsonUtility.ToJson(data));
                }
            }
        }
    }
}
