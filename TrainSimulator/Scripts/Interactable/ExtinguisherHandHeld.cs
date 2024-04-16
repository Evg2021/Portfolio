using System.Collections;
using UnityEngine;

public class ExtinguisherHandHeld : HandHeldInteractable
{
    [SerializeField] private ParticleSystem _foamEffect;
    [SerializeField] private float _animationDuration;

    public override void PlayAnimation()
    {
        base.PlayAnimation();
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        _foamEffect.Play();
        yield return new WaitForSeconds(_animationDuration);
        _foamEffect.Stop();
    }
}
