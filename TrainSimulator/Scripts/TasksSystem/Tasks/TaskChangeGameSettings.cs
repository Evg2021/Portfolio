using UnityEngine;
using NaughtyAttributes;
using Zenject;

public class TaskChangeGameSettings : BaseTask
{
    [ReadOnly][SerializeField] private GameSettingsInstaller _gameSettingsInstaller;
    [ReadOnly][SerializeField] private GameplaySettings _defaultSettings;

    [BoxGroup("Camera")]
    [OnValueChanged("CameraSetDefault")]
    [SerializeField] private bool _changeCamera;

    [BoxGroup("Camera")]
    [ShowIf("_changeCamera")]
    [SerializeField] private CameraSettings _camera = new();

    [BoxGroup("PostFX")]
    [OnValueChanged("PostFXSetDefault")]
    [SerializeField] private bool _changePostFX;

    [BoxGroup("PostFX")]
    [ShowIf("_changePostFX")]
    [SerializeField] private PostProcessingSwitchSettings _postFX = new ();

    [BoxGroup("Outline")]
    [OnValueChanged("OutlineSetDefault")]
    [SerializeField] private bool _changeOutline;

    [BoxGroup("Outline")]
    [ShowIf("_changeOutline")]
    [SerializeField] private OutlineSettings _outline = new ();

    [BoxGroup("Player")]
    [OnValueChanged("PlayerSetDefault")]
    [SerializeField]private bool _changePlayer;

    [BoxGroup("Player")]
    [ShowIf("_changePlayer")]
    [SerializeField] private PlayerSettings _player = new();

    [BoxGroup("Reset settings")]
    [SerializeField] private bool _reset;
    
    private GameplaySettings _settings;

    private void CameraSetDefault() 
    {
        if (_changeCamera)
            _camera = _defaultSettings.Camera;
    }

    private void PostFXSetDefault()
    {
        if (_changePostFX)
            _postFX = _defaultSettings.PostProcessing;
    }

    private void OutlineSetDefault()
    {
        if (_changeOutline)
            _outline = _defaultSettings.Outline;
    }

    private void PlayerSetDefault()
    {
        if (_changePlayer)
            _player = _defaultSettings.Player;
    }

    [Inject]
    private void Construct(GameplaySettings settings)
    {
        _settings = settings;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_gameSettingsInstaller != null) return;
            
        _gameSettingsInstaller = Resources.Load("Data/GameSettings/GameSettingsInstaller") as GameSettingsInstaller;
        _defaultSettings = _gameSettingsInstaller.GameplaySettings.Clone();
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();
        ApplySettings();
        FireCompleteSignal();
    }

    private void ApplySettings()
    {
        if (_changeCamera) _settings.Camera = _reset ? _defaultSettings.Camera : _camera;

        if (_changePostFX) _settings.PostProcessing = _reset ? _defaultSettings.PostProcessing : _postFX;

        if (_changeOutline) _settings.Outline = _reset ? _defaultSettings.Outline : _outline;

        if (_changePlayer) _settings.Player = _reset ? _defaultSettings.Player : _player;
    }
}