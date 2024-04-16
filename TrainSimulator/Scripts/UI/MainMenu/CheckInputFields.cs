using System;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Zenject;

public class CheckInputFields : MonoBehaviour
{
    [SerializeField] private TextRequirements[] _text;
    [SerializeField] private UnityEvent _events;

    private WarningPopup _warningPopup;

    [Inject]
    public void Construct(WarningPopup signalBus)
    {
        _warningPopup = signalBus;
    }

    public void CheckInputFieldsIsEmpty()
    {
        bool isCorrect = true;
        string warningMessage = "";
        
        foreach (TextRequirements text in _text)
        {
            if (!text.IsCorrect(ref warningMessage))
                isCorrect = false;
        }

        if (isCorrect)
        {
            _events.Invoke();
        }
        else
        {
            _warningPopup.Show("Ошибка!", warningMessage);
        }
    }

    [Serializable]
    public class TextRequirements
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private int _minLength;
        [SerializeField] private int _maxLength;
        [SerializeField] private TextRequirementsType _requirementsType;

        private readonly string _namePattern = @"([А-ЯЁ][а-яё]+[\-\s]?){3,}";

        public bool IsCorrect(ref string warningMessage)
        {
            bool result = true;

            switch (_requirementsType)
            {
                case TextRequirementsType.Name:
                    string formattedText = Format(_text.text);

                    if (!CheckLength(formattedText))
                    {
                        result = false;
                        warningMessage += $"Длина ФИО не может быть меньше {_minLength} и больше {_maxLength} символов!\n";
                    }

                    if (!Regex.IsMatch(formattedText, _namePattern))
                    {
                        result = false;
                        warningMessage += $"Некорректное ФИО!\n";
                    }

                    return result;

                case TextRequirementsType.Group:
                    if (!CheckLength(_text.text))
                    {
                        result = false;
                        warningMessage += $"Длина названия группы не может быть меньше {_minLength} и больше {_maxLength} символов!\n";
                    }

                    return result;
            }

            string Format(string text)
            {
                string trimmedInput = Regex.Replace(text, @"\s+", " ");
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string formattedInput = textInfo.ToTitleCase(trimmedInput.ToLower());

                return formattedInput;
            }

            bool CheckLength(string text)
            {
                return (text.Length >= _minLength && text.Length <= _maxLength);
            }

            return true;
        }

        public enum TextRequirementsType
        {
            Name,
            Group
        }
    }
}
