using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Fire : Interactable, IItemInteractable
{
    [SerializeField] private bool _destroyable = true;

    [ShowIf("_destroyable")]
    [SerializeField] private float _destroyDurationMultiplier = 0.5f;

    public void InteractWithItem() => PlayAnimation();

    private void PlayAnimation()
    {
        float startSize = transform.localScale.x;
        float duration = startSize * _destroyDurationMultiplier;

        transform.DOScale(0, duration)
            .SetEase(Ease.InOutCirc)
            .OnComplete(() =>
            {
                if (_destroyable)
                    gameObject.SetActive(false);
                else
                    transform.DOScale(startSize, duration)
                        .SetEase(Ease.OutElastic);
            });  
    }
}