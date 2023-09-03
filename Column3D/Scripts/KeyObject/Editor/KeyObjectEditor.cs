using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeyObject))]
[CanEditMultipleObjects]
public class KeyObjectEditor : Editor
{
    private static GameObject viewPositionsHolder;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var keyObject = (KeyObject)target;
        var sceneCameraTransform = SceneView.lastActiveSceneView.camera.transform;

        GUILayout.Space(10);

        if (GUILayout.Button("Create view point at scene view position"))
        {
            viewPositionsHolder = ScenarioCreator.GetViewPositionsHolder();

            keyObject.CreateViewPoint(sceneCameraTransform.position, sceneCameraTransform.rotation, viewPositionsHolder.transform);
        }

        var creator = FindObjectOfType<ScenarioCreator>();
        if (creator != null)
        {
            var lastStage = creator.LastScenarioStage;
            if (lastStage != null)
            {
                if (GUILayout.Button("Add KeyObject to Last ScenarioStage"))
                {
                    lastStage.KeyObject = keyObject;
                    Selection.activeObject = lastStage;
                }
            }
        }

        if (keyObject.ViewTransform != null)
        {
            if (GUILayout.Button("Set SceneCamera on ViewPoint"))
            {
                SceneView.lastActiveSceneView.rotation = keyObject.ViewRotation;
                SceneView.lastActiveSceneView.AlignViewToObject(keyObject.ViewTransform);
            }
        }
    }
}
