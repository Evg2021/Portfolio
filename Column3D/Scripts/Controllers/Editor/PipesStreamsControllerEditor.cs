using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipesStreamsController))]
public class PipesStreamsControllerEditor : Editor
{
    private static string collectPipesButtonName = "Collect Streams";
    private static string ClearNullPipeKeyObjectsButtinName = "Clear nulls PipeKeyObjects";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var controller = (PipesStreamsController)target;

        if (GUILayout.Button(collectPipesButtonName))
        {
            controller.CollectStreams();
        }
        if (GUILayout.Button(ClearNullPipeKeyObjectsButtinName))
        {
            controller.ClearNullStreams();
        }
    }
}
