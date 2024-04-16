using System;
using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string value)
    {
        _text.text = value;
    }
}