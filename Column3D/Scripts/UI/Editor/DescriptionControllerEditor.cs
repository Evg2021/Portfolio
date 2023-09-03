using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DescriptionController))]
public class DescriptionControllerEditor : Editor
{

    private void OnEnable()
    {
        var controller = (DescriptionController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var controller = (DescriptionController)target;

        if(GUILayout.Button("Rotate Description Window on Camera"))
        {
            var cameraTransform = SceneView.lastActiveSceneView.camera.transform;
            controller.transform.rotation = cameraTransform.rotation;
        }

        GUILayout.Space(10);

        EditorGUIUtility.labelWidth = 60.0f;
        if (!string.IsNullOrEmpty(controller.Message))
        {
            int count = Utility.GetStringParamsCount(controller.Message);
            if (count > 0)
            {
                InitializeArguments(ref controller, count);

                for (int i = 0; i < count; i++)
                {
                    GUILayout.BeginHorizontal();
                    controller.Arguments[i, 0] = EditorGUILayout.FloatField("Value: ", controller.Arguments[i, 0]);
                    controller.Arguments[i, 1] = EditorGUILayout.FloatField("Step: ", controller.Arguments[i, 1]);
                    controller.Arguments[i, 2] = EditorGUILayout.FloatField("Max Value: ", controller.Arguments[i, 2]);
                    controller.IsLoop[i]       = EditorGUILayout.ToggleLeft("Is Loop: ", controller.IsLoop[i]);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    private void InitializeArguments(ref DescriptionController controller, int count)
    {
        if (controller.Arguments == null)
        {
            controller.Arguments = new float[count, 3];
        }

        if (count != controller.Arguments.GetLength(0))
        {
            var oldArguments = controller.Arguments;
            controller.Arguments = new float[count, 3];
            var length = controller.Arguments.GetLength(0) > oldArguments.GetLength(0) ? oldArguments.GetLength(0) : controller.Arguments.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                controller.Arguments[i, 0] = oldArguments[i, 0];
                controller.Arguments[i, 1] = oldArguments[i, 1];
                controller.Arguments[i, 2] = oldArguments[i, 2];
            }
        }

        if (controller.IsLoop == null)
        {
            controller.IsLoop = new bool[count];
        }

        if (controller.IsLoop.Length != count)
        {
            var oldArguments = controller.IsLoop;
            controller.IsLoop = new bool[count];
            var length = controller.IsLoop.Length > oldArguments.Length ? oldArguments.Length : controller.IsLoop.Length;
            for (int i = 0; i < length; i++)
            {
                controller.IsLoop[i] = oldArguments[i];
            }
        }
    }
}
 