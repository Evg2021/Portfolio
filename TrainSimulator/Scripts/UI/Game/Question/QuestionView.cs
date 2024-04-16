using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Zenject;

public class QuestionView : MonoBehaviour, ICursorControllable
{
    public delegate void ClickAnswer();
    public ClickAnswer OnClickCorrectAnswer = delegate { };
    public ClickAnswer OnClickWrongAnswer = delegate { };

    [Header("Components")]
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private Image _image;
    [SerializeField] private Button _acceptButton;

    [Header("Spawn settings")]
    [SerializeField] private Transform _container;
    [SerializeField] private AnswerView _answerPrefab;
    [SerializeField][Min(1)] private int _answerCountDefault = 5;
    [SerializeField][Min(1)] private int _answerCountMax = 10;

    private Dictionary<Answer, AnswerView> _answers = new();
    private ObjectPool<AnswerView> _pool;

    [Inject] private CursorController _cursorController;
    private Answer _selectedAnswer;

    private void Awake()
    {
        _acceptButton.onClick.AddListener(ApplySelectedAnswer);

        _pool = new ObjectPool<AnswerView>(() =>
            Instantiate(_answerPrefab),
            answer => { answer.gameObject.SetActive(true); answer.Initialize(this); },
            answer => answer.gameObject.SetActive(false),
            answer => Destroy(answer.gameObject),
            false,
            _answerCountDefault,
            _answerCountMax
        );

        SetActive(false);
    }

    private void OnDestroy()
    {
        _acceptButton.onClick.RemoveListener(ApplySelectedAnswer);
    }

    public void Show(Question question)
    {
        if (question == null) return;
        if (question.Answers.Count == 0) return;

        SetActive(true);
        _questionText.text = question.Name; 

        foreach (var answer in question.Answers)
        {
            AnswerView element = _pool.Get();
            element.SetAnswer(answer);
            element.transform.SetParent(_container, false);
            _answers.Add(answer, element);
        }

        StartCoroutine(Rebuild());
    }

    private IEnumerator Rebuild()
    {
        yield return null;
        LayoutRebuilder.MarkLayoutForRebuild(transform.parent.GetComponent<RectTransform>());
    }
    
    public void Hide()
    {
        _selectedAnswer = null;

        if (_answers.Count > 0)
        {
            foreach (Answer answer in _answers.Keys)
                _pool.Release(_answers[answer]);

            _answers.Clear();
        }
       
        SetActive(false);
    }

    public void SelectAnswer(Answer answer)
    {
        if (_selectedAnswer == answer)
        {
            _answers[_selectedAnswer].SetSelected(false);
            _selectedAnswer = null;

            return;
        }

        if (_selectedAnswer != null)
            _answers[_selectedAnswer].SetSelected(false);

        _selectedAnswer = answer;
        _answers[_selectedAnswer].SetSelected(true);
    }

    private void ApplySelectedAnswer()
    {
        if (_selectedAnswer == null) return;

        if (_selectedAnswer.IsCorrect)
            OnClickCorrectAnswer?.Invoke();
        else
            OnClickWrongAnswer?.Invoke();
    }

    private void SetActive(bool value)
    {
        _image.enabled = value;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(value);

        if (value)
            _cursorController.AddUser(this);
        else
            _cursorController.RemoveUser(this);
    }
}
