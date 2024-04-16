using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MonitorUPUComponentContainer : MonoBehaviour
{
    [SerializeField] private Speedometr _speedometr;
    private Treadmill _treadmill;

    [Inject]
    private void Construct(Treadmill treadmill)
    {
        _treadmill = treadmill;
        _speedometr.SetTreadmill(_treadmill);
    }

    private void OnDestroy()
    {
        _speedometr.Dispose();
    }
}
