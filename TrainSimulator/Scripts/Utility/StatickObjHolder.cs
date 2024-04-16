using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatickObjHolder", menuName = "Data/StatickObjHolder", order = 1)]
public class StatickObjHolder : ScriptableObject
{
    [HideInInspector] public List<Transform> StaticObjects;
}
