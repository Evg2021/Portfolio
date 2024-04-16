using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TaskItemContainer : BaseTask
{
    [SerializeField] private ItemContainer toolBag;
    [SerializeField] private List<ItemData> _importantItemList;
    private InventoryPanel _inventoryPanel;
    private int _countTools = 0;

    [Inject]
    private void Construct(InventoryPanel inventoryPanel)
    {
        _inventoryPanel = inventoryPanel;
    }
    
    protected override void OnValidate()
    {
        base.OnValidate();

        _softWrongMessages = new();
        _softWrongMessages.Add("Вам это не понадобится!");
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();
        _inventoryPanel.OnItemReceive.AddListener(OnItemGrab);
        _signalBus.Subscribe<StartInteractSignal>(OnInterract);
        _signalBus.Subscribe<StopInteractSignal>(OnInterract);
        toolBag.CanInterract = true;
    }

    private void OnItemGrab(ISlotComponentContainer slot)
    {
        ItemGrab(slot.ItemSlot.StoredItemData);
    }

    public override void HiglightEnable()
    {
        base.HiglightEnable();
        toolBag.HighlightItems(_importantItemList);
    }

    private void OnInterract(InteractableSignal obj)
    {
        if (obj.Value.Interactable != toolBag)
            FireHardWrong();
    }

    private void ItemGrab(ItemData item)
    {
        if (_importantItemList.Contains(item))
        {
            _countTools++;
            if (_importantItemList.Count == _countTools)
            {
                FireCompleteSignal();
            }
        }
        else
        {
            FireSoftWrong(_softWrongMessages[0]);
        }
    }

    public override void Dispose()
    {
        toolBag.CanInterract = false;
        _inventoryPanel.OnItemReceive.RemoveListener(OnItemGrab);
        _signalBus.Unsubscribe<StartInteractSignal>(OnInterract);
        _signalBus.Unsubscribe<StopInteractSignal>(OnInterract);
    }
}