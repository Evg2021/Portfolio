using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class PressureGaugeSubtask
{
    [SerializeField] private float _toValue;
    [SerializeField] private float _duration;
    [SerializeField] private EPressureGaugeSide _pressureGaugeSide;
    [SerializeField] private EGaugeArrow _arrow;
    private SignalBus _signalBus;

    public void LaunchSubtask(SignalBus signalBus)
    {
        _signalBus = signalBus;
        PressureChangesSignal signal = new() { ToValue = _toValue, Duration = _duration, PressureGaugeSide = _pressureGaugeSide, Arrow = _arrow};
        _signalBus.TryFire(signal);
    }
}
public class TaskPressureGauge : TaskAwaiting
{
    [Space(10)] 
    [SerializeField] private List<PressureGaugeSubtask> _pressureGaugeSubtasks;

    private PressureGauge _pressureGauge;

    [Inject]
    private void Construct(PressureGauge pressureGauge)
    {
        _pressureGauge = pressureGauge;
    }
    protected override void LaunchTask()
    {
        base.LaunchTask();
        foreach (var i in _pressureGaugeSubtasks)
        {
            i.LaunchSubtask(_signalBus);
        }
    }

    public override void HiglightEnable()
    {
        base.HiglightEnable();
        _higlightHolder.EnableOultines(_pressureGauge.GetComponent<Outline>());
    }

    public override void HiglightDisable()
    {
        base.HiglightDisable();
        _higlightHolder.DisableAll();
    }
}