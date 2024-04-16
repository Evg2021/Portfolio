using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class TreadmillExamples : MonoBehaviour
{
    private Treadmill _treadmill;
    private float _duration;

    [Inject]
    private void Construct(Treadmill treadmill)
    {
        _treadmill = treadmill;
    }

    public void SetDuration(float value)
    {
        _duration = value;
    }

    public void SetSpeed(float newSpeed)
    {
        _treadmill.SetSpeed(newSpeed, _duration);
    }
}