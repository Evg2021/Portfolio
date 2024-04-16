using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string Name;
    public string Group;
    public GameplayRole Role;
    public GameplayMode Mode;

    [ReadOnly]
    public SituationData SituationData;

    [BoxGroup("Editor only")]
    [OnValueChanged("SetSelectedScenario")]
    [SerializeField] private SituationGroupsData _situationGroupsData;

    [BoxGroup("Editor only")]
    [Dropdown("_groups")]
    [SerializeField] private SituationsGroup _selectedGroup;
    private DropdownList<SituationsGroup> _groups()
    {
        DropdownList<SituationsGroup> displayGroups = new();
        displayGroups.Add("Situation Groups Data is Empty", null);

        if (_situationGroupsData != null)
        {
            displayGroups.Clear();

            foreach (var group in _situationGroupsData.SituationsGroups)
                displayGroups.Add($"{group.Id + 1}. {group.Name}", group);
        }

        return displayGroups;
    }

    [BoxGroup("Editor only")]
    [Dropdown("_situations")]
    [SerializeField] private SituationData _selectedSituation;
    private DropdownList<SituationData> _situations()
    {
        DropdownList<SituationData> displaySituations = new();
        displaySituations.Add("Situation Groups Data is Empty", null);

        if (_situationGroupsData != null)
        {
            displaySituations.Clear();

            int index = 1;

            foreach (var situation in _selectedGroup.Situations)
            {
                displaySituations.Add($"{index}. {situation.Name}", situation);
                index++;
            }
        }

        return displaySituations;
    }

    [Button("Установить выбранный сценарий")]
    private void SetSelectedScenario()
    {
        // Set the new value
        SituationData = _selectedSituation;

        // Output debug message
        Debug.Log("SituationData set to " + _selectedSituation);
    }
}
