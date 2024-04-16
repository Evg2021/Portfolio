using UnityEngine;
using UnityEngine.Events;

public class TaskAction : TaskAwaiting
{
    [SerializeField] private UnityEvent _event;

    protected override void TimerOutAction()
    {
        _event?.Invoke();
        base.TimerOutAction();
    }
}