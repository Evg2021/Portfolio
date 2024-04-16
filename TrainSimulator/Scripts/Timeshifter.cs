using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Timeshifter : MonoBehaviour
{
    public Volume Volume;
    public float Duration = 1;
    private MotionBlur _motionBlur;

    private void Awake()
    {
        if (Volume.profile.TryGet(out MotionBlur motionBlur))
        {
            _motionBlur = motionBlur;
        }
    }

    public void SetTimeSpeed(float value)
    {
        value = Mathf.Clamp01(value);
        var startTimescale = Time.timeScale;
        DOVirtual.Float(startTimescale, value, Duration, f =>
        {
            Time.timeScale = f;
            if (_motionBlur != null)
                _motionBlur.active = !Mathf.Approximately(Time.timeScale, 1);
        });
    }
}