using DG.Tweening;
using UnityEngine;

public class EmergencyBrake : Interactable, IItemInteractable
{
    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private float _duration = 0.3f;
    private Vector3 _closePosition = Vector3.zero;

    public void InteractWithItem()
    {
        Vector3 targetPos = transform.localPosition == _openPosition ? _closePosition : _openPosition;
        transform.DOLocalMove(targetPos, _duration).SetEase(Ease.Linear);
    }
}
