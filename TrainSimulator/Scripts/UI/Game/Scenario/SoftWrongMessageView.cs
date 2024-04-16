using System.Collections;
using TMPro;
using UnityEngine;

public class SoftWrongMessageView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;
    
    private IEnumerator _coroutine;
    
    private void Awake()
    {
        _panel.SetActive(false);
    }

    public void Show(string message)
    {
        _text.text = message;
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = ShowPanel(GetDurationByWordsLenght(message));
        StartCoroutine(_coroutine);
    }

    private IEnumerator ShowPanel(float duration)
    {
        _panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        _panel.SetActive(false);
    }

    private float GetDurationByWordsLenght(string message)
    {
        string[] words = message.Split(' ');
        int wordCount = words.Length;
        float time = wordCount * 50f / 120f;
        return time;
    }
}
