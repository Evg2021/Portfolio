using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HardWrongMessageView : InfoMessageView
{
    [SerializeField] private Image _fading;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _fadingValue;

    private IEnumerator _coroutine;

    public override void Show(InfoMessage message)
    {
        base.Show(message);

        if(_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = HideTimer();
        StartCoroutine(_coroutine);
    }

    public override void Hide()
    {
        base.Hide();

        if(_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(2);
        Hide();
    }
    
}