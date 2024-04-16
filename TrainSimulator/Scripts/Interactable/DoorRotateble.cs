using DG.Tweening;
using UnityEngine;

public class DoorRotateble : Door
{
    [SerializeField] private Vector3 _targetRotation;
    private Vector3 _originRotation;

    private void Awake()
    {
        _originRotation = _door.localRotation.eulerAngles;
        if (_door == null)
        {
            print("Door not assigned!!!");
            _door = transform;
        }
    }

    public override void Open()
    {
        _door.DOLocalRotate(_targetRotation, 2).SetEase(Ease.Linear);
    }

    public override void CloseDoor()
    {
        _door.localRotation = Quaternion.Euler(_originRotation);
    }
}