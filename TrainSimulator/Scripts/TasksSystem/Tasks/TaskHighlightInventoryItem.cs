using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TaskHighlightInventoryItem : BaseTask
{
    [SerializeField] private ItemData _importantItem;
    private InventoryPanel _inventoryPanel;

    [Inject]
    private void Construct(InventoryPanel inventoryPanel)
    {
        _inventoryPanel = inventoryPanel;
    }

    protected override void LaunchTask()
    {
        if (_playerData.Mode == GameplayMode.Exam)
        {
            print("Skip task TaskHighlightInventoryItem for exam");
            FireCompleteSignal();
            return;
        }

        if (_inventoryPanel.GetCurrentItem() == _importantItem)
        {
            FireCompleteSignal();
        }
        else
        {
            base.LaunchTask();
            _signalBus.Subscribe<ClickToolSlotSignal>(ClickToSlot);
        }
    }

    public override void HiglightEnable()
    {
        base.HiglightEnable();
        _inventoryPanel.HighlightingItems(_importantItem);
    }

    public override void HiglightDisable()
    {
        base.HiglightDisable();
        _inventoryPanel.StopHighlightingItem();
    }

    public override void Dispose()
    {
        base.Dispose();
        _signalBus.TryUnsubscribe<ClickToolSlotSignal>(ClickToSlot);
    }

    private void ClickToSlot(ClickToolSlotSignal obj)
    {
        if (_importantItem == obj.Value.ItemSlot.StoredItemData)
        {
            FireCompleteSignal();
        }
        else
        {
            FireSoftWrong(_softWrongMessages[0]);
        }
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        base.OnStartInteract(signal);
        FireHardWrong();
    }
}