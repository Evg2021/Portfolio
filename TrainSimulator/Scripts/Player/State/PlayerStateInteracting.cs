using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerStateInteracting : PlayerState, ICursorControllable
{
    private SignalBus _signalBus;
    private Player _player;
    private Settings _settings;
    private CursorController _cursorController;

    private RaycastEntity _selectedEntity = null;
    private RaycastEntity _currentEntity = null;

    private Interactable _currentInteractable;
    private LayerMask _ignoreLayer = ~(1 << 2);
    private Vector3 _hitPoint;

    public PlayerStateInteracting(SignalBus signalBus, GameplaySettings settings, Player player,
        CursorController cursorController)
    {
        _signalBus = signalBus;
        _player = player;
        _settings = settings.Player.StateInteracting;
        _cursorController = cursorController;
    }

    public override void Start()
    {
        _cursorController.AddUser(this);

        _player.Input.Enable();
        _player.Input.PlayerWASD.Fire1.started += StartInteract;
        _player.Input.PlayerWASD.Fire1.canceled += StopInteract;
        _player.Input.PlayerWASD.Fire2.canceled += ExitState;
        _player.Input.PlayerWASD.Observe.performed += Interact;

        _player.PlayerCamera.SetOriginRotation();
    }

    public override void Update()
    {
        if (_currentEntity != null) return;
        
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Ray ray = _player.PlayerCamera.ScreenPointToRay(pointerPosition);
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();

        if (!isOverUI && Physics.Raycast(ray, out RaycastHit hit, _settings.InteractDistance, _ignoreLayer) &&
            hit.collider.TryGetComponent(out RaycastEntity entity))
        {
            _hitPoint = hit.point;
            SelectEntity(entity);
        }
        else
            DeselectEntity();
    }

    private void SelectEntity(RaycastEntity entity)
    {
        if (_selectedEntity == entity) return;

        DeselectEntity();

        _selectedEntity = entity;
        _signalBus.Fire(new EnterPointerEntitySignal() { Value = _selectedEntity });
    }

    private void DeselectEntity()
    {
        // if (_selectedEntity == null)
        //     return;

        _selectedEntity = null;
        _signalBus.Fire(new ExitPointerEntitySignal() { });
    }

    private void StartInteract(InputAction.CallbackContext context)
    {
        if (_selectedEntity == null) return;

        _currentEntity = _selectedEntity;

        if (_selectedEntity.Interactable != null)
        {
            if (_selectedEntity.Interactable.CanInterract)
            {
                _currentInteractable = _selectedEntity.Interactable;
                StartInteractable();
            }

            _signalBus.Fire(new StartInteractSignal() { Value = _selectedEntity });
        }

        if (_selectedEntity.ZoomAt != null)
            Zoom(true);
    }

    private void StartInteractable()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.StartInteract();
        }
    }

    private void Interact(InputAction.CallbackContext contex)
    {
        if (_currentInteractable != null)
        {
            _signalBus.Fire(new MoveInteractSignal() { Value = _selectedEntity });
            _currentInteractable.Interact(contex);
        }
    }

    private void StopInteract(InputAction.CallbackContext context)
    {
        DeselectEntity();

        if (_currentEntity == null) return;

        if (_currentEntity.Interactable != null)
            StopInteractable();

        if (_currentEntity.ZoomAt != null)
            Zoom(false);

        _currentInteractable = null;
        _currentEntity = null;
    }

    private void StopInteractable()
    {
        if (_currentInteractable != null)
        {
            _signalBus.Fire(new StopInteractSignal() { Value = _currentEntity });
            _currentInteractable.EndInteract();
        }
    }

    private void ExitState(InputAction.CallbackContext context)
    {
        if (_player.ChangeState(PlayerStates.Moving))
            StopInteract(context);
    }

    private void Zoom(bool enabled)
    {
        if (enabled)
            _player.PlayerCamera.ZoomAt(_currentEntity, _hitPoint);
        else
            _player.PlayerCamera.ResetZoom();
    }

    public override void Dispose()
    {
        _cursorController.RemoveUser(this);

        _player.Input.PlayerWASD.Fire1.started -= StartInteract;
        _player.Input.PlayerWASD.Fire1.canceled -= StopInteract;
        _player.Input.PlayerWASD.Fire2.canceled -= ExitState;
        _player.Input.PlayerWASD.Observe.performed -= Interact;
        _player.Input.Disable();
    }

    [Serializable]
    public class Settings : InstallerSettings
    {
        public float InteractDistance;
    }

    public class Factory : PlaceholderFactory<PlayerStateInteracting> { }
}