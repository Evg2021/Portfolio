using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerStateMoving : PlayerState
{
    public PlayerStateMoving(GameplaySettings settings, Player player)
    {
        _settings = settings.Player.StateMoving;
        _player = player;
    }

    private Player _player;
    private Settings _settings;
    private float _xRotation = 0f;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private float _speed;
    private BodyCondition _bodyCondition;

    public override void Start()
    {
        _player.Input.Enable();
        SubscribeInputs();

        float angle = _player.PlayerCamera.transform.rotation.eulerAngles.x;
        if (angle > 180f)
            angle -= 360f;

        _xRotation = angle;

        SetSpeedIsCrouching();
    }

    private void SubscribeInputs()
    {
        _player.Input.PlayerWASD.Fire2.performed += SetInteractingState;
        _player.Input.PlayerWASD.Crouch.started += SwitchCrouching;
        _player.Input.PlayerWASD.MoveAcceleration.started += StartRun;
        _player.Input.PlayerWASD.MoveAcceleration.canceled += StopRun;
        _player.Input.PlayerWASD.Jump.started += Jump;
    }

    private void UnsubscribeInputs()
    {
        _player.Input.PlayerWASD.Fire2.performed -= SetInteractingState;
        _player.Input.PlayerWASD.Crouch.started -= SwitchCrouching;
        _player.Input.PlayerWASD.MoveAcceleration.started -= StartRun;
        _player.Input.PlayerWASD.MoveAcceleration.canceled -= StopRun;
        _player.Input.PlayerWASD.Jump.started -= Jump;
    }

    public override void Dispose()
    {
        UnsubscribeInputs();
        _player.Input.Disable();
        _player.Body.Move(Vector3.zero);
    }

    public override void Update()
    {
        GetInputs();
        Move();
        MouseLook();
    }

    private void GetInputs()
    {
        _movementInput = _player.Input.PlayerWASD.Move.ReadValue<Vector2>();
        _lookInput = _player.Input.PlayerWASD.Observe.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 forward = _player.Body.Forward * _movementInput.y;
        Vector3 right = _player.Body.Right * _movementInput.x;
        Vector3 movement = (forward + right).normalized;

        _player.Body.Move(movement * _speed);
    }

    private void MouseLook()
    {
        float lookX = _lookInput.x * _settings.MouseSensitivity * Time.fixedDeltaTime;
        float lookY = _lookInput.y * _settings.MouseSensitivity * Time.fixedDeltaTime;

        _xRotation -= lookY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _player.PlayerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        _player.Body.Rotate(Vector3.up * lookX);
    }

    private void StartRun(InputAction.CallbackContext context) => StopCrouching(BodyCondition.Run);

    private void StopRun(InputAction.CallbackContext context) => SetConditionSpeed(BodyCondition.Default);

    private void SwitchCrouching(InputAction.CallbackContext context)
    {
        _player.Body.SwitchCrouch();
        SetSpeedIsCrouching();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_player.Body.Jump() && _bodyCondition != BodyCondition.Crouch) return;

        StopCrouching(BodyCondition.Default);
    }

    private void StopCrouching(BodyCondition condition)
    {
        _player.Body.Crouch(false);
        SetConditionSpeed(condition);
    }

    private void SetSpeedIsCrouching() => SetConditionSpeed(_player.Body.IsCrouch ? BodyCondition.Crouch : BodyCondition.Default);

    private void SetInteractingState(InputAction.CallbackContext context) => _player.ChangeState(PlayerStates.Interacting);

    private void SetConditionSpeed(BodyCondition newCondition)
    {
        _bodyCondition = newCondition;

        switch (_bodyCondition)
        {
            case BodyCondition.Default:
                _speed = _settings.DefaultSpeed;
                return;

            case BodyCondition.Run:
                _speed = _settings.RunSpeed;
                return;

            case BodyCondition.Crouch:
                _speed = _settings.CrouchSpeed;
                return;
        }
    }

    private enum BodyCondition { Default, Run, Crouch }

    [Serializable]
    public class Settings : InstallerSettings
    {
        public float MouseSensitivity;
        public float DefaultSpeed;
        public float CrouchSpeed;
        public float RunSpeed;
    }

    public class Factory : PlaceholderFactory<PlayerStateMoving> { }
}
