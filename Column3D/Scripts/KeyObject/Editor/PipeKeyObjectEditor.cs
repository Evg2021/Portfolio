using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeKeyObject))]
[CanEditMultipleObjects]
public class PipeKeyObjectEditor : KeyObjectEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var pipe = (PipeKeyObject)target;
        if (pipe.TryGetComponent<Collider>(out _))
        {
            if (GUILayout.Button("Update Streams Data"))
            {
                pipe.UpdateStreamsData();
            }
        }
    }
}
