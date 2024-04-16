using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(PlayerBody))]
public class Player : MonoBehaviour
{
    public PlayerCamera PlayerCamera => _camera;
    public PlayerBody Body => _body;
    public PlayerInput Input => _input;
   
    [SerializeField] private PlayerCamera _camera;
    [SerializeField] private PlayerBody _body;

    private SignalBus _signalBus;
    private PlayerStateFactory _stateFactory;
    private PlayerInput _input;
    private PlayerState _state;

    private bool _lockStateSwitching = false;

    private bool _isFreeze;

    [Inject]
    private void Construct(SignalBus signalBus, PlayerStateFactory stateFactory, PlayerInput input)
    {
        _signalBus = signalBus;
        _stateFactory = stateFactory;
        _input = input;
    }

    public void Start()
    {
        ChangeState(PlayerStates.Moving);

        _signalBus.Subscribe<GamePauseSignal>(HandlePause);
    }

    private void OnDestroy()
    {
        if (_state != null)
            _state.Dispose();

        _signalBus.Unsubscribe<GamePauseSignal>(HandlePause);
    }

    private void Update()
    {
        if (_isFreeze) return;

        _state.Update();
    }

    private void FixedUpdate()
    {
        if (_isFreeze) return;

        _state.FixedUpdate();
    }

    private void LateUpdate()
    {
        if (_isFreeze) return;

        _state.LateUpdate();
    }

    public bool ChangeState(PlayerStates state)
    {
        if (_state != null)
        {
            if (_lockStateSwitching) return false;
            _state.Dispose();
        }
            
        _state = _stateFactory.CreateState(state);
        _state.Start();

        _signalBus.Fire(new PlayerStateChangeSignal() { Value = state });

        return true;
    }

    public void SetLockStateSwitching(bool value)
    {
        _lockStateSwitching = value;
    }

    private void HandlePause(GamePauseSignal signal)
    {
        _isFreeze = signal.Value;
    }
}