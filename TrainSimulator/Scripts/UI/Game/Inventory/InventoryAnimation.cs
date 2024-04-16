using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Zenject;

public class InventoryAnimation : MonoBehaviour
{
    [SerializeField] private float _hidePosX = -1150;
    [SerializeField] private RectTransform _movableRect;

    private float _showPosX;
    private bool _isShow = false;
    private bool _inMoveProcess = false;
    
    private PlayerInput _playerInput;
    private SignalBus _signalBus;
    private SelectedToolView _selectedToolView;

    [Inject]
    private void Construct(PlayerInput playerInput, SignalBus signalBus, SelectedToolView selectedToolView)
    {
        _selectedToolView = selectedToolView;
        _signalBus = signalBus;
        _playerInput = playerInput;
        
        _playerInput.PlayerWASD.Inventory.started += AnimationLaunch;
        _selectedToolView.ToolButton.onClick.AddListener(AnimationLaunch);
        _signalBus.Subscribe<StartInteractSignal>(InteractableWith);
    }



    private void OnDestroy()
    {
        _playerInput.PlayerWASD.Inventory.started -= AnimationLaunch;
        _selectedToolView.ToolButton.onClick.RemoveListener(AnimationLaunch);
        _signalBus.TryUnsubscribe<StartInteractSignal>(InteractableWith);
    }

    private void Start()
    {
        _showPosX = transform.localPosition.x;
        ResetPosition();
    }

    private void ResetPosition()
    {
        var localPosition = _movableRect.localPosition;
        localPosition = new Vector3(_hidePosX, localPosition.y, localPosition.z);
        _movableRect.localPosition = localPosition;
    }
    
    private void InteractableWith(StartInteractSignal obj)
    {
        if(_isShow)
            AnimationLaunch();
    }
    
    private void AnimationLaunch(InputAction.CallbackContext obj)
    {
        AnimationLaunch();
    }
    private void AnimationLaunch()
    {
        if (_inMoveProcess)
            return;

        _inMoveProcess = true;
        _isShow = !_isShow;
        float newX = _isShow ? _showPosX : _hidePosX;

        _movableRect.DOLocalMoveX(newX, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _inMoveProcess = false; 
        });
    }
}

