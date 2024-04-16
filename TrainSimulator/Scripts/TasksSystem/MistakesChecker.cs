using System;
using UnityEngine;

[Serializable]
public class MistakesChecker
{
    public int MaxOfMistakes;
    private int _mistakesCount = 0;
    public int MistakeCount => _mistakesCount;
    
    public void Reset()
    {
        _mistakesCount = 0;
    }
    public bool AddMistakeAndCheck()
    {
        _mistakesCount++;
        Debug.Log("Mistake! " + (MaxOfMistakes - _mistakesCount) + " is left!");
        return _mistakesCount >= MaxOfMistakes;
    }
}