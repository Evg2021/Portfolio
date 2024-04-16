using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Question
{
    public string Name => _name;
    public IReadOnlyCollection<Answer> Answers => _answers;

    [SerializeField][TextArea] private string _name;
    [SerializeField] private List<Answer> _answers = new();
}