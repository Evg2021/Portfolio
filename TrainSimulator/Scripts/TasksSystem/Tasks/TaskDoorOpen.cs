using UnityEngine;

public class TaskDoorOpen : TaskInteract
{
    [SerializeField] private ItemData _doorKey;

    protected override void InterractWithRightEntity(RaycastEntity entity)
    {
        if (_inventoryPanel.GetCurrentItem() == _doorKey || _doorKey == null)
        {
            if (entity.Interactable.TryGetComponent(out IItemInteractable itemInteractable))
            {
                itemInteractable.InteractWithItem();
                base.InterractWithRightEntity(entity);
            }
            else
            {
                Debug.LogError(entity.name+ " - Current entity not have IItemInteract interface");
            }
        }
        else
        {
            FireSoftWrong(_softWrongMessages[0]);
        }
    }
}