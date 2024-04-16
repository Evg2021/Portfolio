using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SituationsGroup
{
    public string Name => _name;
    public Sprite Preview => _preview;
    public string Description => _description;
    public int Id => _id;
    public IReadOnlyList<SituationData> Situations => _situations;

    [SerializeField][TextArea] private string _name;
    [SerializeField] private Sprite _preview;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private int _id;
    [SerializeField] private SituationData[] _situations;
}