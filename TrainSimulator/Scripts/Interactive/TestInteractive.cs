using UnityEngine;
using DG.Tweening;
using System;

public class TestInteractive : Interactive
{
    public float scaleDuration = 0.2f;
    public Vector3 pressedScale = new (0.9f, 0.9f, 0.9f);
    public Vector3 normalScale = new (1f, 1f, 1f);

    public override void OnInteractTick(object value)
    {
        if (Convert.ToBoolean(value))
            OnPressed();
        else
            OnReleased();
    }

    private void OnPressed()
    {
        transform.DOScale(pressedScale, scaleDuration);
    }

    private void OnReleased()
    {
        transform.DOScale(normalScale, scaleDuration);
    }
}