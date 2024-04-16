using UnityEngine;
using Zenject;

public class TaskAddItem : BaseTask
{
    [SerializeField] private ItemData _item;
    private InventoryPanel _inventoryPanel;

    [Inject]
    private void Construct(InventoryPanel inventoryPanel)
    {
        _inventoryPanel = inventoryPanel;
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();
        _inventoryPanel.CreateSlot(_item);
        FireCompleteSignal();
    }
}