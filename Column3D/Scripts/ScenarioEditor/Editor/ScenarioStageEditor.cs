using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioStage))]
public class ScenarioStageEditor : Editor
{
    private static string OnStartStageName = "OnStartStage";
    private static string OnEndStageName = "OnEndStage";
    private static float CurveResolution = 100;
    private static float defaultUIElementDistance = 1.0f;

    private bool isTransition;

    public override void OnInspectorGUI()
    {
        ScenarioStage stage = (ScenarioStage)target;

        var style = EditorStyles.helpBox;
        style.fontStyle = FontStyle.Bold;
        GUILayout.Space(10);
        GUILayout.Label("Please, put only objects with KeyObject component", style);
        base.OnInspectorGUI();

        isTransition = stage.IsTransitionalStage;

        if (!stage.IsTransitionalStage)
        {
            GUILayout.Space(10);
            GUILayout.Label("Description Window will be created like standalone KeyObject.");

            if (GUILayout.Button("Add Description Window"))
            {
                var descriptionPrefab = Utility.GetDescriptionWindow3DPrefab();
                var canvas = Utility.GetCanvas3D();

                if (canvas == null || descriptionPrefab == null)
                {
                    return;
                }

                var rotation = SceneView.lastActiveSceneView.rotation;
                var position = SceneView.lastActiveSceneView.camera.transform.forward * defaultUIElementDistance + SceneView.lastActiveSceneView.camera.transform.position;

                var newDescription = Instantiate(descriptionPrefab, position, rotation, canvas.transform);
                newDescription.name = Utility.DescriptionWindow3DPrefabName + (canvas.transform.childCount - 1);

                if (stage.DescriptionWindows == null)
                {
                    stage.DescriptionWindows = new List<DescriptionController>();
                }

                if (newDescription.TryGetComponent<DescriptionController>(out var controller))
                {
                    stage.DescriptionWindows.Add(controller);
                    Selection.activeObject = controller;
                }
                else
                {
                    Debug.LogError("Prefab \"" + Utility.DescriptionWindow3DPrefabName + "\" has no DescriptionController component. New Description Window was not added in Description Windows List.");
                }
            }
            if (GUILayout.Button("Add Gradient Object"))
            {
                var gradientPrefab = Utility.GetGradientObjectPrefab();
                var canvas = Utility.GetCanvas3D();

                if (canvas == null || gradientPrefab == null)
                {
                    return;
                }

                var rotation = SceneView.lastActiveSceneView.rotation;
                var position = SceneView.lastActiveSceneView.camera.transform.forward * defaultUIElementDistance + SceneView.lastActiveSceneView.camera.transform.position;

                var newGradient = Instantiate(gradientPrefab, position, rotation, canvas.transform);
                newGradient.name = Utility.GradientPrefabName + (canvas.transform.childCount - 1);

                if (stage.GradientObjects == null)
                {
                    stage.GradientObjects = new List<GradientController>();
                }

                if (newGradient.TryGetComponent<GradientController>(out var controller))
                {
                    stage.GradientObjects.Add(controller);
                    Selection.activeObject = controller;
                }
                else
                {
                    Debug.LogError("Prefab \"" + Utility.GradientPrefabName + "\" has no DescriptionController component. New Description Window was not added in Description Windows List.");
                }
            }

            if (stage.KeyObject != null)
            {
                if (stage.KeyObject.ViewTransform != null)
                {
                    if (GUILayout.Button("Set SceneCamera on ViewPoint"))
                    {
                        SceneView.lastActiveSceneView.rotation = stage.KeyObject.ViewRotation;
                        SceneView.lastActiveSceneView.AlignViewToObject(stage.KeyObject.ViewTransform);
                    }
                }
            }
        }
        else
        {
            if (GUILayout.Button("Create View Point for Transition Stage"))
            {
                var sceneCameraTransform = SceneView.lastActiveSceneView.camera.transform;
                stage.transform.position = sceneCameraTransform.position;
                stage.transform.rotation = sceneCameraTransform.rotation;
            }            
        }
    }

    public void OnSceneGUI()
    {
        if (isTransition)
        {
            ScenarioStage stage = (ScenarioStage)target;

            var previousStage = stage.GetPrevousStage();
            var nextStage = stage.GetNextStage();

            if (previousStage != null && nextStage != null)
            {
                if (previousStage.KeyObject != null && nextStage.KeyObject != null)
                {
                    var transitionPosition = stage.transform.position;
                    var previousStagePosition = previousStage.KeyObject.ViewPosition;
                    var nextStagePosition = nextStage.KeyObject.ViewPosition;

                    var transitionRotation = stage.transform.rotation;
                    var previousStageRotation = previousStage.KeyObject.ViewRotation;
                    var nextStageRotation = nextStage.KeyObject.ViewRotation;

                    for (int i = 0; i < CurveResolution; i++)
                    {
                        float value = (i + 1) / CurveResolution;
                        float nextValue = (i + 2) / CurveResolution;
                        var position = Vector3.Lerp(Vector3.Lerp(previousStagePosition, transitionPosition, value),
                                                    Vector3.Lerp(transitionPosition, nextStagePosition, value), value);
                        var nextPosition = Vector3.Lerp(Vector3.Lerp(previousStagePosition, transitionPosition, nextValue),
                                                        Vector3.Lerp(transitionPosition, nextStagePosition, nextValue), nextValue);
                        var rotation = Quaternion.Lerp(Quaternion.Lerp(previousStageRotation, transitionRotation, value),
                                                       Quaternion.Lerp(transitionRotation, nextStageRotation, value), value);
                        Handles.SphereHandleCap(i, position, Quaternion.identity, 0.1f, EventType.Repaint);
                        Handles.DrawLine(position, nextPosition);

                        Handles.ArrowHandleCap(i, position, rotation, 1.0f, EventType.Repaint);
                    }
                }
            }
        }
    }
}
