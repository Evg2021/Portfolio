using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimePassedFading : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _text;

    private Color _imageColor;
    private Color _textColor;

    private void Start()
    {
        _textColor = _text.color;
        _textColor.a = 0f;
        _imageColor = _image.color;
        _imageColor.a = 0f;

        _text.color = _textColor;
        _image.color = _imageColor;
    }

    public void Show(string message, float duration)
    {
        _text.text = message;
        _image.raycastTarget = true;

        float animationPartDuration = duration / 2f;

        _image.DOFade(1f, animationPartDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                _text.DOFade(1f, animationPartDuration)
                    .SetEase(Ease.Linear);
            });
    }

    public void Hide(float duration)
    {
        float animationPartDuration = duration / 2f;

        _text.DOFade(0f, animationPartDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                _image.DOFade(0f, animationPartDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => _image.raycastTarget = false);
            });
    }
}
