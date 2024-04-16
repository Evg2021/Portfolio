using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WarningPopup : MonoBehaviour
{
    [SerializeField] private GameObject _popup;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private TMP_Text _content;
    [SerializeField] private Image _fadingImage;

    [Header("Animation values")]
    [SerializeField] private float _fadeValue = 0.85f;
    [SerializeField] private float _hidenHeight = -450f;
    [SerializeField] private float _animationShowDuration = 0.2f;
    [SerializeField] private float _animationHideDuration = 0.2f;

    public void Show(string header, string content)
    {
        _header.text = header;
        _content.text = content;

        _popup.transform.DOLocalMoveY(0, _animationShowDuration);
        _fadingImage.raycastTarget = true;
        _fadingImage.DOFade(_fadeValue, _animationShowDuration);
    }

    public void Hide()
    {
        _popup.transform.DOLocalMoveY(_hidenHeight, _animationHideDuration);
        _fadingImage.DOFade(0f, _animationHideDuration)
            .OnComplete(() => _fadingImage.raycastTarget = false);
    }
}
