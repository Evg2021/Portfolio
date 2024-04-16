using Zenject;

public class TaskInteract : TaskEntity
{
    protected InventoryPanel _inventoryPanel;

    [Inject]
    private void Construct(InventoryPanel inventoryPanel)
    {
        _inventoryPanel = inventoryPanel;
    }
    protected override void LaunchTask()
    {
        base.LaunchTask();

        foreach (var i in _entities)
            i.Interactable.CanInterract = true;
    }

    public override void Dispose()
    {
        base.Dispose();

        foreach (var i in _entities)
            i.Interactable.CanInterract = false;
    }
}