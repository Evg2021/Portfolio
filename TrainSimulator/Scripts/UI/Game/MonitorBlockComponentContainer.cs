using UnityEngine;
using Zenject;


public class MonitorBlockComponentContainer : MonoBehaviour
{
    [SerializeField] private Speedometr _speedometr;
    [SerializeField] private PressureLines _pressureLines;
    
    private Treadmill _treadmill;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(Treadmill treadmill, SignalBus signalBus)
    {
        _signalBus = signalBus;
        _treadmill = treadmill;
        _speedometr.SetTreadmill(_treadmill);
        _pressureLines.Initing(_signalBus);
    }

    private void OnDestroy()
    {
        _speedometr.Dispose();
        _pressureLines.Dispose();
    }
}
