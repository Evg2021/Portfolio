using UnityEngine;
using Zenject;

public class TaskPickObject : TaskEntity
{
    [SerializeField] private ItemData _itemForReceive;
    private InventoryPanel _inventoryPanel;

    [Inject]
    private void Construct(InventoryPanel inventoryPanel)
    {
        _inventoryPanel = inventoryPanel;
    }

    protected override void InterractWithRightEntity(RaycastEntity entity)
    {
        entity.gameObject.SetActive(false);
        _inventoryPanel.CreateSlot(_itemForReceive);
        base.InterractWithRightEntity(entity);
    }
}