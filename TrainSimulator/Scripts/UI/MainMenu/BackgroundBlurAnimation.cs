using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackgroundBlurAnimation : MonoBehaviour
{
    [SerializeField] private Image _blurImage;

    [Header("Anim settings")]
    [SerializeField] private float _duration;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        DOTween.Kill(_blurImage, true);
        _blurImage.DOFade(1f, _duration);
    }

    public void Hide()
    {
        DOTween.Kill(_blurImage, true);
        _blurImage.DOFade(0f, _duration);
    }
}
