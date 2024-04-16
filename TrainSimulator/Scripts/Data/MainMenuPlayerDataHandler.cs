using TMPro;
using UnityEngine;
using Zenject;

public class MainMenuPlayerDataHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameText;
    [SerializeField] private TMP_InputField _groupText;
    [SerializeField] private bool _setDataOnStart;

    private PlayerData _playerData;

    [Inject]
    public void Construct(PlayerData playerData)
    {
        _playerData = playerData;
    }

    private void Start()
    {
        if (!_setDataOnStart) return;

        _nameText.text = _playerData.Name;
        _groupText.text = _playerData.Group;
    }

    public void SetName()
    {
        _playerData.Name = _nameText.text;
    }

    public void SetGroup()
    {
        _playerData.Group = _groupText.text;
    }

    public void SetSituationData(SituationData data)
    {
        _playerData.SituationData = data;
    }

    public void SetGameplayRole(GameplayRole role)
    {
        _playerData.Role = role;
    }

    public void SetGameplayMode(GameplayMode mode)
    {
        _playerData.Mode = mode;
    }
}