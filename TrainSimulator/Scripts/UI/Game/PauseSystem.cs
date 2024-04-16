using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseSystem : MonoBehaviour, ICursorControllable
{
    [SerializeField] private GameObject _pauseWindow;
    [SerializeField] private GameObject _hintWindow;
    [SerializeField] private Button _exitPause;
    [SerializeField] private Button _reloadScenario;
    [SerializeField] private Button _exitMenu;

    private SignalBus _signalBus;
    private PlayerInput _playerInput;
    private CursorController _cursorController;
    private TransitionWindow _sceneSwitch;

    private bool _isPaused = false;

    [Inject] 
    public void Construct(SignalBus signalBus, PlayerInput playerInput, CursorController cursorController, TransitionWindow sceneSwitch)
    {
        _signalBus = signalBus;
        _playerInput = playerInput;
        _cursorController = cursorController;
        _sceneSwitch = sceneSwitch;
    }

    private void Start()
    {
        _exitPause.onClick.AddListener(OnButtonExitPauseClick);
        _reloadScenario.onClick.AddListener(OnButtonReloadScenarioClick);
        _exitMenu.onClick.AddListener(OnButtonExitMenuClick);
        _playerInput.PlayerWASD.Menu.started += OnChangePause;

        SetPause(false);
    }

    private void OnDestroy()
    {
        _exitPause.onClick.RemoveListener(OnButtonExitPauseClick);
        _reloadScenario.onClick.RemoveListener(OnButtonReloadScenarioClick);
        _exitMenu.onClick.RemoveListener(OnButtonExitMenuClick);
        _playerInput.PlayerWASD.Menu.started -= OnChangePause;
    }

    private void OnChangePause(InputAction.CallbackContext context)
    {
        SetPause(!_isPaused);
    }

    private void OnButtonExitPauseClick()
    {
        SetPause(false);
    }

    private void OnButtonExitMenuClick()
    {
        _sceneSwitch.LoadScene("MainMenu", true);
    }

    private void OnButtonReloadScenarioClick()
    {
        _sceneSwitch.LoadScene("GameScene", true);
    }

    private void SetPause(bool pause)
    {
        _isPaused = pause;
        _pauseWindow.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0.0f : 1.0f;

        _signalBus.Fire(new GamePauseSignal() { Value = _isPaused }); 

        if (_isPaused)
            _cursorController.AddUser(this);
        else
            _cursorController.RemoveUser(this);

        if (_hintWindow.activeSelf)
            _hintWindow.SetActive(false);
    }
}