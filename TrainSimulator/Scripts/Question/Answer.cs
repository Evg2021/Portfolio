using System;
using UnityEngine;

[Serializable]
public class Answer
{
    public string Text => _text;
    public bool IsCorrect => _isCorrect;

    [SerializeField][TextArea] private string _text;
    [SerializeField] private bool _isCorrect;
}