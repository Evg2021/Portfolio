using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class TransitionWindow : MonoBehaviour
{
    [SerializeField] private Image _fadingImage;
    [SerializeField] private GameObject _loadingWindow;
    [SerializeField] private LoadingProgress _loadingProgress;
    [SerializeField] private Color _fadeInColor;
    [SerializeField] private float _fadingDuration;

    private AsyncOperation _asyncOperation = null;
    private string _loadingSceneName;

    private CursorController _cursorController;

    private bool _onlyFading = false;

    [Inject]
    private void Construct(CursorController cursorController)
    {
        _cursorController = cursorController;
    }

    private void Start()
    {
        var transparentColor = _fadeInColor;
        transparentColor.a = 0f;

        _fadingImage.color = transparentColor;
        _loadingWindow.SetActive(false);

        _loadingProgress.OnEndAnimationComplete += OnCompleteLoading;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        _loadingProgress.OnEndAnimationComplete -= OnCompleteLoading;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        Time.timeScale = 1f;

        _fadingImage.DOFade(0f, _fadingDuration)
            .SetUpdate(true);
    }

    public void LoadScene(string loadingSceneName, bool onlyFading = false)
    {
        _loadingSceneName = loadingSceneName;
        _onlyFading = onlyFading;

        _cursorController.ClearUsers();

        _fadingImage.DOFade(1f, _fadingDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (!_onlyFading)
                {
                    _loadingWindow.SetActive(true);

                    _fadingImage.DOFade(0f, _fadingDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        _loadingProgress.StartLoadingAnimation();
                        StartCoroutine(LoadSceneAsync(_loadingSceneName));
                    });
                }
                else
                    StartCoroutine(LoadSceneAsync(_loadingSceneName));
            });
    }

    private void OnCompleteLoading()
    {
        if (!_onlyFading)
            _fadingImage.DOFade(1f, _fadingDuration)
                .SetUpdate(true)
                .OnComplete(() => 
                { 
                    Complete();
                    _loadingWindow.SetActive(false);
                });
        else
            Complete();

        void Complete()
        {
            DOTween.Clear(true);
            _asyncOperation.allowSceneActivation = true;
        }
    }

    private IEnumerator LoadSceneAsync(string loadingSceneName)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(loadingSceneName);
        _asyncOperation.allowSceneActivation = false;

        bool completeLoading = false;

        while (!_asyncOperation.isDone)
        {
            if (!completeLoading && _asyncOperation.progress >= 0.9f)
            {
                completeLoading = true;

                if (!_onlyFading)
                    _loadingProgress.EndLoadingAnimation();
                else
                    OnCompleteLoading();
            }

            yield return null;
        }
    }
}