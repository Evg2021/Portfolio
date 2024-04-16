using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

[System.Serializable]
public class PressureLines
{
    [SerializeField] private Image _TC_line;
    [SerializeField] private Image _TM_line;
    [SerializeField] private Image _UR_line;
    private SignalBus _signalBus;

    public void Initing(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<PressureChangesSignal>(OnPressureChange);
    }

    private void OnPressureChange(PressureChangesSignal obj)
    {
        float value = obj.ToValue / 16;
        if (obj.PressureGaugeSide == EPressureGaugeSide.LEFT)
        {
            if (obj.Arrow == EGaugeArrow.RED)
                _TM_line.DOFillAmount(value, obj.Duration).SetEase(Ease.Linear);
            else
                _UR_line.DOFillAmount(value, obj.Duration).SetEase(Ease.Linear);
        }
        else
        {
            if (obj.Arrow == EGaugeArrow.RED)
                _TC_line.DOFillAmount(value, obj.Duration).SetEase(Ease.Linear);
        }
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<PressureChangesSignal>(OnPressureChange);
    }
}