using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneCameraMoving : EditorWindow
{
    private Vector3 sceneCameraPosition;
    private GameObject go;
    private Transform transform;

    [MenuItem("T-Soft/Utilities/Move scene camera")]
    private static void Init()
    {
        SceneCameraMoving sceneCameraMoving = (SceneCameraMoving)EditorWindow.GetWindow(typeof(SceneCameraMoving));
        sceneCameraMoving.Show();
    }

    private void OnGUI()
    {
        sceneCameraPosition = EditorGUILayout.Vector3Field("Enter XYZ for moving scene camera", sceneCameraPosition);
        
        if (GUILayout.Button("Apply"))
        {
            if (!go)
            {
                go = new GameObject();
            }
            if (go)
            {
                sceneCameraPosition = new Vector3(sceneCameraPosition.x, sceneCameraPosition.y + 1.7f, sceneCameraPosition.z);
                go.transform.position = sceneCameraPosition;
                SceneView.lastActiveSceneView.AlignViewToObject(go.transform);

                DestroyImmediate(go);
            }
        }
    }
}
