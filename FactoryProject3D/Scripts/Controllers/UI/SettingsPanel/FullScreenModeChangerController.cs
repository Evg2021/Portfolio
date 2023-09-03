using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenModeChangerController : MonoBehaviour, IParameter
{
    private bool currentFullScreenMode;
    private Toggle windowModeToggle;

    private void Awake()
    {
        currentFullScreenMode = Screen.fullScreen;

        InitializeWindowModeToggle();
    }

    private void InitializeWindowModeToggle()
    {
        windowModeToggle = GetComponentInChildren<Toggle>();

        if (windowModeToggle)
        {
            windowModeToggle.SetIsOnWithoutNotify(currentFullScreenMode);
        }
        else
        {
            Debug.LogError("UI Initialization Error: " + transform.name + " has not toggle object in children.");
        }
    }

    public void OnModeChange(bool value)
    {
        currentFullScreenMode = value;
    }

    public bool IsParameterChanged()
    {
        return currentFullScreenMode != Screen.fullScreen;
    }

    public void Apply()
    {
        Settings.SetFullScreen(currentFullScreenMode);
    }
}
