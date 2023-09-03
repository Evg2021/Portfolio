using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefectsList" ,menuName = "Create Deffects List")]
public class DefectsList : SingletonScriptableObject<DefectsList>
{
    public List<DefectItem> DefectsItems;
}

[Serializable]
public struct DefectItem
{
    public string DefectName;
    public GameObject Effect;
}