using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuController))]
public class MenuControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var menu = (MenuController)target;

        if (GUILayout.Button("Refresh Scnearios List"))
        {
            menu.RefreshScenariosList();
        }
    }
}
