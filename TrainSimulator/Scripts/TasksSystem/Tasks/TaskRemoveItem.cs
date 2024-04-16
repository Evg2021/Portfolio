using UnityEngine;
using Zenject;

public class TaskRemoveItem : BaseTask
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
        _inventoryPanel.DeleteItemBy(_item);
        FireCompleteSignal();
    }
}