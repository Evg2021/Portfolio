using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScenarioCreator : SingletonEditor<ScenarioCreator>
{
    [Header("Scenario Stages:")]
    public List<ScenarioStage> Stages;

    private static GameObject keyObjectViewPositionsHolder;

    private static string scenarioStageName = "ScenarioStage";
    private static string keyObjectViewPositionsHolderName = "KeyObjectViewPositions";

    public ScenarioStage LastScenarioStage
    {
        get
        {
            if(transform.childCount > 0)
            {
                return transform.GetChild(transform.childCount - 1).GetComponent<ScenarioStage>();
            }

            return null;
        }
    }

    public List<ScenarioStageStruct> GetStageStructuries()
    {
        var result = new List<ScenarioStageStruct>();
        var count = transform.childCount;

        for(int i = 0; i < count; i++)
        {
            var scenarioStage = transform.GetChild(i).GetComponent<ScenarioStage>();
            result.Add(Utility.GetScenarioStageStruct(scenarioStage));
        }

        return result;
    }

    public void AddStage(ScenarioStage stage)
    {
        if (Stages == null)
            Stages = new List<ScenarioStage>();

        Stages.Add(stage);
    }

    public List<Track> GetTracks()
    {
        var keyObjects = new List<KeyObject>();
        var stages = transform.childCount;

        for(int i = 0; i < stages; i++)
        {
            keyObjects.Add(transform.GetChild(i).GetComponent<ScenarioStage>().KeyObject);
        }

        return GetTracks(keyObjects);
    }

    public List<Track> GetTracks(List<KeyObject> keyObjects)
    {
        if (keyObjects == null || keyObjects.Count == 0) return null;

        var tracks = new List<Track>();
        foreach(var keyObject in keyObjects)
        {
            if (keyObject != null)
            {
                tracks.Add(new Track()
                {
                    ViewPosition = keyObject.ViewPosition,
                    ViewRotation = keyObject.ViewRotation
                });
            }
        }

        return tracks;
    }

    public void LoadScenarioAsset(ScenarioAsset asset)
    {
        for(int i = 0; i < asset.Stages.Count; i++)
        {
            var newStage = CreateScenarioStage(scenarioStageName);
            Utility.InitializeScenarioStage(newStage, asset.Stages[i]);
        }
    }

    public static ScenarioStage CreateScenarioStage(Transform transform, string scenarioStageName)
    {
        var stage = new GameObject(scenarioStageName + transform.childCount);
        var stageComponent = stage.AddComponent<ScenarioStage>();
        stage.transform.parent = transform;

        return stageComponent;
    }

    public ScenarioStage CreateScenarioStage(string scenarioStageName)
    {
        var stage = new GameObject(scenarioStageName + transform.childCount);
        var stageComponent = stage.AddComponent<ScenarioStage>();
        stage.transform.parent = transform;

        if(Stages == null)
        {
            Stages = new List<ScenarioStage>();
        }

        Stages.Add(stageComponent);

        return stageComponent;
    }

    public void RemoveStages()
    {
        Stages = new List<ScenarioStage>();
        var stagesCount = transform.childCount;

        if (stagesCount == 0) return;

        for(int i = 0; i < stagesCount; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public static GameObject GetViewPositionsHolder()
    {
        if (keyObjectViewPositionsHolder == null || keyObjectViewPositionsHolder.transform == null)
        {
            keyObjectViewPositionsHolder = GameObject.Find(keyObjectViewPositionsHolderName);
            if (keyObjectViewPositionsHolder == null || keyObjectViewPositionsHolder.transform == null)
            {
                keyObjectViewPositionsHolder = new GameObject(keyObjectViewPositionsHolderName);
            }
        }

        return keyObjectViewPositionsHolder;
    }
}
