using System;
using UnityEngine;

[Serializable]
public class InfoMessage
{
    public string Name => _name;
    public string Content => _content;

    [SerializeField]
    [TextArea]
    private string _name;

    [SerializeField]
    [TextArea]
    private string _content;

    public InfoMessage(string name, string content)
    {
        _name = name;
        _content = content;
    }
}