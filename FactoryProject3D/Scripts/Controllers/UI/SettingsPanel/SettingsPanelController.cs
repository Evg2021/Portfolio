using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    public GameObject ConfirmationWindow;

    private IParameter[] parameters;

    private void Awake()
    {
        InitializeConfirmationWindow();

        parameters = GetComponentsInChildren<IParameter>();
    }
    private void InitializeConfirmationWindow()
    {
        if (ConfirmationWindow && ConfirmationWindow.activeSelf)
        {
            ConfirmationWindow.SetActive(false);
        }
    }

    public void OnClickApply()
    {
        foreach (var param in parameters)
        {
            if (param.IsParameterChanged())
            {
                param.Apply();
            }
        }

        Settings.SaveSettings();
    }

    public void OnClickBack()
    {
        if (ConfirmationWindow && WasSettingsChanged())
        {
            ConfirmationWindow.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private bool WasSettingsChanged()
    {
        foreach (var param in parameters)
        {
            if (param.IsParameterChanged())
            {
                return true;
            }
        }

        return false;
    }
}
