using UnityEngine;

public class TaskEndValve : TaskInteract
{
    [SerializeField] private int _targetValue;
    [SerializeField] private ItemData _requiredItem;

    protected override void OnValidate()
    {
        base.OnValidate();

        _softWrongMessages = new();
        _softWrongMessages.Add("Не верное положение рычага");
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        foreach (var i in _entities)
            if (signal.Value.Interactable != i.Interactable)
                FireHardWrong();
            else
                if (_inventoryPanel.GetCurrentItem() != _requiredItem)
                {
                    print("Не тот инструмент!");
                    FireHardWrong();
                    i.Interactable.GetComponent<Switch>().ReturnCondition();
                }
    }

    protected override void OnStopInterract(InteractableSignal signal)
    {
        foreach (var i in _entities)
            if (signal.Value.Interactable == i.Interactable)
                if (i.Interactable.GetComponent<Switch>().GetValue() == _targetValue)
                    FireCompleteSignal();
                else
                    FireSoftWrong(_softWrongMessages[0]);
            else
                FireHardWrong();
    }
}