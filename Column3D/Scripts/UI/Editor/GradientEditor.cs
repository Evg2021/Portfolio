using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GradientController))]
public class GradientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var controller = (GradientController)target;

        if (GUILayout.Button("Rotate Description Window on Camera"))
        {
            var cameraTransform = SceneView.lastActiveSceneView.camera.transform;
            controller.transform.rotation = cameraTransform.rotation;
        }
    }
}
