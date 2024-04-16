using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PixelPlay.OffScreenIndicator;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RadialCheckBar : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _progressBar;

    private bool _isLaunch = false;
    private Tweener _tweener;

    private void Awake()
    {
        Reset();
    }

    public void Launch(float duration)
    {
        _isLaunch = true;
        _panel.SetActive(true);
        _tweener = _progressBar.DOFillAmount(1, duration).SetEase(Ease.Linear);
    }

    private void LateUpdate()
    {
        if (_isLaunch)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Reset()
    {
        _isLaunch = false;
        _tweener.Kill();
        _progressBar.fillAmount = 0;
        _panel.SetActive(false);
    }
}
