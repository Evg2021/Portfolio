using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class SituationsList : MonoBehaviour
{
    [SerializeField] private UnityEvent _showGroupSituationsEvent;

    [Header("Situations")]
    [SerializeField] private SelectSituationButton _situationButtonPrefab;
    [SerializeField] private Transform _situationsButtonsContainer;

    [Header("Situations Groups")]
    [SerializeField] private ButtonSelectSituationGroup _situationsGroupButtonPrefab;
    [SerializeField] private Transform _situationsGroupButtonsContainer;

    [Header("Situations Groups Description settings")]
    [SerializeField] private TMP_Text _descriptionHeader;
    [SerializeField] private Image _descriptionImage;
    [SerializeField] private TMP_Text _descriptionContent;

    [Header("Data")]
    [SerializeField] private SituationGroupsData _situationsData;
    [SerializeField] private MainMenuPlayerDataHandler _playerDataHandler;

    private List<SituationsGroup> _situationsGroups = new();
    private List<SituationButton> _situationsButtons = new();

    private SituationsGroup _selectedGroup;
    private TransitionWindow _sceneSwitch;

    [Inject]
    public void Construct(TransitionWindow sceneSwitch)
    {
        _sceneSwitch = sceneSwitch;
    }

    private void Start()
    {
        SetData();
        SetSituationsGroupDescription(0);
    }

    public void SetSituationsGroupDescription(int groupId)
    {
        _selectedGroup = _situationsGroups[groupId];

        _descriptionHeader.text = _selectedGroup.Name;
        _descriptionImage.sprite = _selectedGroup.Preview;
        _descriptionContent.text = _selectedGroup.Description;
    }

    private void SetData()
    {
        _situationsGroups.AddRange(_situationsData.SituationsGroups);

        foreach (var situationsGroup in _situationsGroups)
        {
            CreateSituationsGroup(situationsGroup, _situationButtonPrefab,
                _situationsButtonsContainer, _situationsGroupButtonPrefab, _situationsGroupButtonsContainer);
        }
    }

    public void ShowSelectedGroupSituations()
    {
        _showGroupSituationsEvent.Invoke();

        foreach (var _ in _situationsGroups)
            GroupSetActive(_selectedGroup.Id);
    }

    private void CreateSituationsGroup(
        SituationsGroup group, SelectSituationButton situationButtonPrefab, 
        Transform situationButtonContainer, ButtonSelectSituationGroup groupButtonPrefab, 
        Transform groupButtonContainer)
    {
        ButtonSelectSituationGroup groupButton = Instantiate(groupButtonPrefab, groupButtonContainer);
        groupButton.SetButton(group.Id, group.Name, group.Preview, this);

        foreach (var situation in group.Situations)
        {
            SelectSituationButton situationButton = Instantiate(situationButtonPrefab, situationButtonContainer);
            situationButton.SetButton(this, situation);
            _situationsButtons.Add(new(group.Id, situationButton));
        }
    }

    private void GroupSetActive(int groupId)
    {
        foreach (SituationButton situationButton in _situationsButtons)
            situationButton.Button.gameObject.SetActive(situationButton.GroupId == groupId);
    }

    public void OnClickSituationButton(SelectSituationButton button)
    {
        SituationData data = button.SituationData;

        if (data == null) return;

        _sceneSwitch.LoadScene(data.SceneName);
        _playerDataHandler.SetSituationData(data);
    }

    public class SituationButton
    {
        public int GroupId => _groupId;
        public SelectSituationButton Button => _button;

        private readonly int _groupId;
        private readonly SelectSituationButton _button;

        public SituationButton(int id, SelectSituationButton situationButton)
        {
            _groupId = id;
            _button = situationButton;
        }
    }
}
