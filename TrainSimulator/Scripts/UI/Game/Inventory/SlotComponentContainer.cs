using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public interface ISlotComponentContainer
{
    public void Initialize(InventorySettings inventorySettings);
    public IItemSlot ItemSlot { get; }
    public RectangleOutline RectangleOutline { get; }
    public Transform TransformSelf { get; }
    public void Destroy();
    public UnityEvent<ISlotComponentContainer> OnClickSlot { get; }
    public UnityEvent<ISlotComponentContainer> OnEnterSlot { get; }
    public UnityEvent<ISlotComponentContainer> OnExitSlot { get; }
}

public class SlotComponentContainer : MonoBehaviour, ISlotComponentContainer
{
    public Transform TransformSelf => transform;
    public IItemSlot ItemSlot
    {
        get { return _itemSlot; }
    }

    public RectangleOutline RectangleOutline
    {
        get { return _rectangleOutline; }
    }
    private ItemSlot _itemSlot;
    private RectangleOutline _rectangleOutline;
    public UnityEvent<ISlotComponentContainer> OnClickSlot { get; } = new();
    public UnityEvent<ISlotComponentContainer> OnEnterSlot { get; } = new();
    public UnityEvent<ISlotComponentContainer> OnExitSlot { get; } = new();

    private void OnValidate()
    {
        if (TryGetComponent(out ItemSlot itemSlot))
            _itemSlot = itemSlot;
        if (TryGetComponent(out RectangleOutline rectangleOutline))
            _rectangleOutline = rectangleOutline;
    }

    public void Initialize(InventorySettings inventorySettings)
    {
        _rectangleOutline = GetComponent<RectangleOutline>();
        _itemSlot = GetComponent<ItemSlot>();
        
        _rectangleOutline.Init(_itemSlot, inventorySettings);
        
        Subscribes();
    }

    private void Subscribes()
    {
        _itemSlot.OnClick.AddListener(ClickSLot);
        _itemSlot.OnEnter.AddListener(EnterPointerSlot);
        _itemSlot.OnExit.AddListener(ExitPointerSlot);
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _itemSlot.OnClick.RemoveListener(ClickSLot);
        _itemSlot.OnEnter.RemoveListener(EnterPointerSlot);
        _itemSlot.OnExit.RemoveListener(ExitPointerSlot);
    }

    private void ClickSLot(IItemSlot itemSlot)
    {
        OnClickSlot?.Invoke(this);
    }

    private void ExitPointerSlot(IItemSlot iSlot)
    {
        OnExitSlot?.Invoke(this);
    }

    private void EnterPointerSlot(IItemSlot iSlot)
    {
        OnEnterSlot?.Invoke(this);
    }
    

}