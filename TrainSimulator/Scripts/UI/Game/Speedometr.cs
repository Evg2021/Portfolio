using TMPro;
using UnityEngine;

[System.Serializable]
public class Speedometr
{
    [SerializeField] private Transform _arrow;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private float _minAngle;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    
    private Treadmill _treadmill;

    public void SetTreadmill(Treadmill treadmill)
    {
        _treadmill = treadmill;
        _treadmill.Speed.OnChanged += OnSpeedChanged;
    }
    public void Dispose()
    {
        _treadmill.Speed.OnChanged -= OnSpeedChanged;
    }
    private void OnSpeedChanged(float speed)
    {
        ArrowMove(speed);
        SetSpeedDisplay(speed);
    }

    private void SetSpeedDisplay(float speed)
    {
        _speedText.text = Mathf.Floor(speed).ToString();
    }

    private void ArrowMove(float speed)
    {
        float alpha = (speed - _minSpeed) / (_maxSpeed - _minSpeed);
        float newZ = Mathf.Lerp(_minAngle, _maxAngle, alpha);
        var newRotation = Quaternion.Euler(_arrow.localRotation.x, _arrow.localRotation.y, newZ);
        _arrow.localRotation = newRotation;
    }
}