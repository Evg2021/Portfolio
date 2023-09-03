using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Scenario", menuName = "Create Scenario")]
public class ScenarioAsset : ScriptableObject
{
    public List<ScenarioStageStruct> Stages;
}
