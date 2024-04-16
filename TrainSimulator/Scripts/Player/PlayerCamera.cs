using UnityEngine;
using DG.Tweening;
using Zenject;

public class PlayerCamera : MonoBehaviour
{
    public float fieldOfView
    {
        get => _cam.fieldOfView;
        set => _cam.fieldOfView = value;
    }

    public Camera Camera => _cam;
    [SerializeField] private Camera _cam;

    private Vector3 _originRotation;
    private RaycastEntity _currentEntity;
    private SignalBus _signalBus;
    private GameplaySettings _settings;

    [Inject]
    public void Construct(SignalBus signalBus, GameplaySettings settings)
    {
        _signalBus = signalBus;
        _settings = settings;
    }

    private void Start()
    {
        _cam.fieldOfView = _settings.Camera.DefaultFOV;
        _originRotation = transform.eulerAngles;
    }

    public Ray ScreenPointToRay (Vector2 pos) => _cam.ScreenPointToRay(pos);

    public void SetOriginRotation()
    {
        _originRotation = transform.eulerAngles;
    }

    public void ZoomAt(RaycastEntity entity, Vector3 zoomPoint)
    {
        float distance = Vector3.Distance(transform.position, zoomPoint);
        float heightAtDist = _settings.Camera.ZoomSize / _settings.Camera.DefaultFOV;
        float zoomSize = FOVForHeightAndDistance(heightAtDist, distance);

        _cam.DOFieldOfView(zoomSize, _settings.Camera.ZoomAtDuration);
        _cam.transform.DOLookAt(zoomPoint, _settings.Camera.ZoomAtDuration);
        _currentEntity = entity;
        _signalBus.Fire(new StartZoomAtSignal() { Value = _currentEntity });

        static float FOVForHeightAndDistance(float height, float distance)
        {
            return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
        }
    }

    public void ResetZoom()
    {
        DOTween.Kill(_cam, true);
        DOTween.Kill(_cam.transform, true);

        _cam.DOFieldOfView(_settings.Camera.DefaultFOV, _settings.Camera.ResetZoomDuration);
        _cam.transform.DORotate(_originRotation, _settings.Camera.ResetZoomDuration);
        _signalBus.Fire(new StopZoomAtSignal() { Value = _currentEntity });
    }
}