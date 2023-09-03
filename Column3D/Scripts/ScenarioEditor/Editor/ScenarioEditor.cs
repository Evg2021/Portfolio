using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioCreator))]
public class ScenarioEditor : Editor
{
    private string pathToCameraScenarioSave = "./Assets/Resources/Scenarios";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        var scenarioComponent = (ScenarioCreator)target;

        if(GUILayout.Button("Create Scenario Stage"))
        {
            CreateScenarioStage(scenarioComponent);
        }

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Load Scenario Asset"))
        {
            var absPath = EditorUtility.OpenFilePanel("Открыть ...", Application.dataPath, "asset");
            var relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
            if(relativePath != null && relativePath.Length > 0)
            {
                scenarioComponent.RemoveStages();
                RemoveScenarioObjects();
                var asset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(ScenarioAsset)) as ScenarioAsset;
                scenarioComponent.LoadScenarioAsset(asset);
            }
        }

        if(GUILayout.Button("Generate Scenario Stages Configuration"))
        {
            var absPath = EditorUtility.SaveFilePanel("Сохранить в ...", Application.dataPath, ObjectsNames.ScenarioAssetName + '.' + ObjectsNames.ScenarioAssetExtension, ObjectsNames.ScenarioAssetExtension);
            var relativePath = absPath.Substring(absPath.IndexOf("Assets/"));
            if (relativePath != null && relativePath.Length > 0)
            {
                CreateScenarioStagesConfiguration(scenarioComponent, relativePath);
                scenarioComponent.RemoveStages();
                RemoveScenarioObjects();
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Remove Scenario"))
        {
            scenarioComponent.RemoveStages();
            RemoveScenarioObjects();
        }
    }

    private void CreateScenarioStage(ScenarioCreator creator)
    {
        var stage = creator.CreateScenarioStage(ObjectsNames.ScenarioStageName);
        Selection.activeObject = stage;
    }

    private void CreateScenarioStagesConfiguration(ScenarioCreator creator, string path)
    {
        var asset = CreateInstance<ScenarioAsset>();
        asset.Stages = creator.GetStageStructuries();

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private void RemoveScenarioObjects()
    {
        //RemoveKeyObjectViewPositions();
        RemoveUIElemetsFromCanvas3D();
    }
    private void RemoveKeyObjectViewPositions()
    {
        var holder = ScenarioCreator.GetViewPositionsHolder();
        if (holder != null)
        {
            DestroyImmediate(holder);
        }
    }
    private void RemoveUIElemetsFromCanvas3D()
    {
        var holder = Utility.GetCanvas3D();
        if (holder != null)
        {
            int count = holder.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(holder.transform.GetChild(0).gameObject);
            }
        }
    }

    [MenuItem("GameObject/Scenario/Create ScenarioEditor")]
    public static void CreateScenarioEditor()
    {
        var scenarioEditor = GameObject.Find(ObjectsNames.ScenarioEditorName);
        if(scenarioEditor == null)
        { 
            scenarioEditor = new GameObject(ObjectsNames.ScenarioEditorName);
            scenarioEditor.AddComponent<ScenarioCreator>();
        }

        Selection.activeObject = scenarioEditor;
    }
    [MenuItem("GameObject/Scenario/Create ScenarioPlayer")]
    public static void CreateScenarioPlayer()
    {
        var scenarioPlayer = GameObject.Find(ObjectsNames.ScenarioPlayerName);
        if (scenarioPlayer == null)
        {
            scenarioPlayer = new GameObject(ObjectsNames.ScenarioPlayerName);
            scenarioPlayer.AddComponent<ScenarioPlayer>();
        }

        Selection.activeObject = scenarioPlayer;
    }
}
