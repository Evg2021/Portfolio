using System;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _button;
    [SerializeField] private Rectangle _outline;

    private Answer _answer;
    private QuestionView _questionView;

    public void Initialize(QuestionView questionView)
    {
        _questionView = questionView;
    }

    private void OnEnable()
    {
        SetSelected(false);
    }

    public void SetAnswer(Answer answer)
    {
        _answer = answer;
        _text.text = _answer.Text;
    }

    public void SetSelected(bool value)
    {
        _outline.enabled = value;
    }
    
    public void OnClick()
    {
        _questionView.SelectAnswer(_answer);
    }
}