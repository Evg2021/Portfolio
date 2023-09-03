using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IPChangerController : MonoBehaviour, IParameter
{
    private TMP_InputField inputField;

    private Image inputBackground;
    private Color originalColor;

    private static Color errorColor;
    private static string errorMesage = "Неверно введен IP адрес.";

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeInputField();
        InitializeBackground();
    }
    private void InitializeInputField()
    {
        inputField = GetComponentInChildren<TMP_InputField>();

        if (inputField && !string.IsNullOrEmpty(Settings.IPAddress))
        {
            inputField.text = Settings.IPAddress;
        }
        else if (!inputField)
        {
            Debug.LogError("UI Initialization Error: " + transform.name + " has no TextMeshPro-InputField in children.");
        }
    }
    private void InitializeBackground()
    {
        inputBackground = GetComponentInChildren<Image>();

        if (inputBackground)
        {
            originalColor = inputBackground.color;
        }
        else
        {
            Debug.LogError("UI Initialization Error: " + transform.name + " has no Image in children.");
        }
    }

    public void OnEndChangingData(string value)
    {
        if (inputField && !ValidateIPAddress(value))
        {
            inputField.text = Settings.IPAddress;
            
            if (StartUIManager.Instance)
            {
                StartUIManager.Instance.ShowNotification(errorMesage);
            }
        }
    }

    private bool ValidateIPAddress(string value)
    {
        return !string.IsNullOrEmpty(value) && value.Split('.').Length == 4;
    }

    public bool IsParameterChanged()
    {
        return inputField.text != Settings.IPAddress;
    }

    public void Apply()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            Settings.SetIPAddress(inputField.text);
        }
    }
}
