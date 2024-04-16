using UnityEngine;
using DG.Tweening;

public class DoorMovable : Door
{
    [SerializeField] private Vector3 _openPosition;
    [Space(10)] 
    [SerializeField] private Transform _secondDoor;
    [SerializeField] private Vector3 _secondOpenPosition;
    [Space(10)]
    [SerializeField] private float _duration;
    [SerializeField] private Collider _collider;
    private Vector3 _closePosition;
    private Vector3 _secondClosePosition;

    private void Awake()
    {
        if (_door == null)
            _door = transform;
        
        _closePosition = _door.localPosition;
        if (_secondDoor)
            _secondClosePosition = _secondDoor.localPosition;
    }

    [ContextMenu("Open")]
    public override void Open()
    {
        _door.DOLocalMove(_openPosition, _duration).SetEase(Ease.Linear);
        if (_secondDoor)
            _secondDoor.DOLocalMove(_secondOpenPosition, _duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                _collider.enabled = false;
            });
    }

    public override void CloseDoor()
    {
        _door.localPosition = _closePosition;
        if (_secondDoor)
        {
            _secondDoor.localPosition = _secondClosePosition;
            _collider.enabled = true;
        }
    }
}