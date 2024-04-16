using UnityEngine;
using Zenject;
using DG.Tweening;

public class TaskSetTreadmillSpeed : TaskAwaiting
{
    [SerializeField] private float _value;
    [SerializeField] private Ease _ease;

    private Treadmill _treadmill;

    [Inject]
    private void Construct(Treadmill treadmill)
    {
        _treadmill = treadmill;
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();

        _treadmill.SetSpeed(_value, _timerAwait, _ease);
    }
}
