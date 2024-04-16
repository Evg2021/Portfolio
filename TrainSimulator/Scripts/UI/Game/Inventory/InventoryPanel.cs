using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using System.Linq;
using UnityEngine.Events;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Transform _toolPanel;
    [SerializeField] private Transform _clothPanel;

    [SerializeField] private InventorySettings _inventorySettings;
    [SerializeField] private SlotComponentContainer _slotComponentContainerPrefab;
    public List<ISlotComponentContainer> Slots { get; private set; } = new();
    public UnityEvent<ISlotComponentContainer> OnItemReceive { get; } = new();
    public UnityEvent<ISlotComponentContainer> OnItemDelete { get; } = new();

    private SignalBus _signalBus;
    private ISlotComponentContainer _currentSlotContainer;
    private ItemContainerPanel _itemContainerPanel;

    [Inject]
    private void Construct(SignalBus signalBus, ItemContainerPanel itemContainerPanel)
    {
        _itemContainerPanel = itemContainerPanel;
        _signalBus = signalBus;

        InitPanel();
        Subscribes();
    }

    private void Subscribes()
    {
        _itemContainerPanel.OnItemRemove.AddListener(ItemReceive);
    }

    private void OnDestroy()
    {
        _itemContainerPanel.OnItemRemove.RemoveListener(ItemReceive);
    }

    public ItemData GetCurrentItem()
    {
        return _currentSlotContainer.ItemSlot.StoredItemData;
    }

    public void SelectDefaultSlot()
    {
        SetSelectedSlot(Slots.First());
    }

    private void InitPanel()
    {
        Slots.AddRange(GetComponentsInChildren<ISlotComponentContainer>());
        foreach (var slot in Slots)
        {
            InitSlot(slot);
        }

        _currentSlotContainer = Slots.First();
    }

    private void InitSlot(ISlotComponentContainer slot)
    {
        slot.Initialize(_inventorySettings);
        slot.OnClickSlot.AddListener(SetSelectedSlot);
    }

    private void SetSelectedSlot(ISlotComponentContainer iSlotContainer)
    {
        if (_currentSlotContainer != iSlotContainer)
        {
            if (_currentSlotContainer != null)
                _currentSlotContainer.RectangleOutline.MakeOutlineHide();

            _currentSlotContainer = iSlotContainer;
            _signalBus.Fire(new ClickToolSlotSignal() { Value = _currentSlotContainer });
        }
    }

    public void DeleteCurrentItem()
    {
        Slots.Remove(_currentSlotContainer);
        Destroy(_currentSlotContainer.TransformSelf.gameObject);
        OnItemDelete?.Invoke(_currentSlotContainer);

        SetSelectedSlot(Slots.First());
    }

    public void DeleteItemBy(ItemData itemData)
    {
        ISlotComponentContainer targetSlot = Slots.FirstOrDefault(x => x.ItemSlot.StoredItemData == itemData);
        if (targetSlot != null)
        {
            Slots.Remove(targetSlot);
            Destroy(targetSlot.TransformSelf.gameObject);
            OnItemDelete?.Invoke(targetSlot);
            SetSelectedSlot(Slots.First());
        }
        else
        {
            print("Target item is not exist!");
        }
    }

    private void ItemReceive(ISlotComponentContainer slot)
    {
        if (slot.ItemSlot.StoredItemData.ItemType == EItemType.Tool)
            OnToolSlotReceiving(slot);
        else if (slot.ItemSlot.StoredItemData.ItemType == EItemType.Cloth)
            OnClothSlotReceiving(slot);

        slot.OnClickSlot.AddListener(SetSelectedSlot);
        Slots.Add(slot);
        OnItemReceive?.Invoke(slot);
    }

    private void OnClothSlotReceiving(ISlotComponentContainer slot)
    {
        slot.ItemSlot.BlockedInterract(true);
        MoveSlotToPanel(slot, _clothPanel);
        Destroy(slot.RectangleOutline);
    }

    private void OnToolSlotReceiving(ISlotComponentContainer slot)
    {
        MoveSlotToPanel(slot, _toolPanel);
    }

    private void MoveSlotToPanel(ISlotComponentContainer container, Transform parent)
    {
        container.TransformSelf.parent = parent;
        container.RectangleOutline.MakeOutlineHide();
        container.RectangleOutline.StopFlickering();
    }

    public void HighlightingItems(ItemData importantItem)
    {
        foreach (var i in Slots)
        {
            if (i.ItemSlot.StoredItemData == importantItem)
            {
                i.RectangleOutline.Flickering();
                break;
            }
        }
    }

    public void StopHighlightingItem()
    {
        foreach (var i in Slots)
        {
            i.RectangleOutline.StopFlickering();
        }
    }

    public void CreateSlot(ItemData itemData)
    {
        Transform parent = itemData.ItemType == EItemType.Tool ? _toolPanel : _clothPanel;
        ISlotComponentContainer slot = Instantiate(_slotComponentContainerPrefab, parent);

        slot.Initialize(_inventorySettings);
        slot.ItemSlot.StoredItemData = itemData;
        ItemReceive(slot);
    }
}