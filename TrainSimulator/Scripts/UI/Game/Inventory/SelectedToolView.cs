using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SelectedToolView : MonoBehaviour
{
    public Button ToolButton => _button;

    [SerializeField] private Image _toolImage;
    [SerializeField] private Button _button;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<ClickToolSlotSignal>(ChangeImage);
    }


    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ClickToolSlotSignal>(ChangeImage);
    }
    
    private void ChangeImage(ClickToolSlotSignal signal)
    {
        _toolImage.sprite = signal.Value.ItemSlot.StoredItemData.Sprite;
    }
}