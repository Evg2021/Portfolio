using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSettingsSwitcher : MonoBehaviour
{
    [BoxGroup("Player Data")]
    [Expandable][SerializeField] private PlayerData _playerData;

    [BoxGroup("Settings")]
    [Expandable][SerializeField] private GameSettingsInstaller _defaultSettings;
    [BoxGroup("Settings")]
    [SerializeField] private List<SpecificSettings> _specificSituationSettings;

    private SituationData _currentSituationData;

    private SceneContext _sceneContext;

    [Button]
    public void UpdateEditor()
    {
        bool _specific = false;
        _currentSituationData = _playerData.SituationData;

        if (_specificSituationSettings.Count == 0) return;
    
        foreach (var settings in _specificSituationSettings)
        {
            if (_currentSituationData == settings.SituationData
                && (settings.Role == GameplayRole.Any
                || _playerData.Role == settings.Role))
            {
                SetInstaller(settings.Settings);
                _specific = true;
            }
        }

        if (!_specific)
            SetInstaller(_defaultSettings);

        void SetInstaller(ScriptableObjectInstaller installer)
        {
            List<ScriptableObjectInstaller> _installers = new();
            _installers.Add(installer);
            _sceneContext.ScriptableObjectInstallers = _installers;

            Debug.Log($"Scene settings has changed to {installer.name}");
        }
    }

    private void OnValidate()
    {
        if (_sceneContext == null)
            _sceneContext = GetComponent<SceneContext>();

        UpdateEditor();
    }

    [Serializable]
    public class SpecificSettings
    {
        public string Name => _situationData.Name;
        public SituationData SituationData => _situationData;
        public GameSettingsInstaller Settings => _settings;
        public GameplayRole Role => _role;

        [SerializeField] private SituationData _situationData;
        [Expandable][SerializeField] private GameSettingsInstaller _settings;
        [SerializeField] private GameplayRole _role;
    }
}