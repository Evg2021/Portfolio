using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingProgress : MonoBehaviour
{
    public delegate void AnimationComplete();

    public event AnimationComplete OnEndAnimationComplete = delegate { };

    [Header("Rects")] [SerializeField] private RectTransform _loadingProgressBar;
    [SerializeField] private RectTransform _progress;
    [SerializeField] private RectTransform _progressPointer;

    [Space(5)]
    [SerializeField] private Image _railway;
    [SerializeField] private Image _train;

    [Header("Animation")] 
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _trainMovingDuration = 2f;

    private float _progressWidth;
    private float _progressPointerWidth;

    private Tweener _railwayFading;

    private void Start()
    {
        _progressPointerWidth = (_progressPointer.rect.width * _progressPointer.localScale.x);
        _progressWidth = _loadingProgressBar.rect.width + _progressPointerWidth;

        RectTransformExtension.SetLeft(_loadingProgressBar, -_progressPointerWidth * 0.5f);
        RectTransformExtension.SetRight(_loadingProgressBar, -_progressPointerWidth * 0.5f);

        ClearProgress();
    }

    public void StartLoadingAnimation()
    {
        ClearProgress();

        _railwayFading = _railway
            .DOFade(1f, _fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                DOVirtual.Float(0, 1f, _trainMovingDuration, (x) => SetLoadingProgress(x))
                .SetUpdate(true)
                .SetLoops(-1, LoopType.Restart);
            }); 
    }

    public void EndLoadingAnimation()
    {
        _railwayFading.Kill();

        _train.DOFade(0f, _fadeDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        _railway
            .DOFade(0f, _fadeDuration)
            .SetUpdate(true)
            .OnComplete(() => OnEndAnimationComplete?.Invoke());
    }

    private void SetLoadingProgress(float progress)
    {
        RectTransformExtension.SetLeft(_progress, progress * _progressWidth);
    }

    private void ClearProgress()
    {
        DOTween.Kill(_railway);
        DOTween.Kill(_train);

        Color transparent = Color.white;
        transparent.a = 0f;

        _railway.color = transparent;
        _train.color = Color.white;

        SetLoadingProgress(0f);
    }
}