using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SituationGroups", menuName = "Data/SituationGroupsData")]
public class SituationGroupsData : ScriptableObject
{
    public IReadOnlyList<SituationsGroup> SituationsGroups => _situationsGroups;
    [SerializeField] private List<SituationsGroup> _situationsGroups = new();
}