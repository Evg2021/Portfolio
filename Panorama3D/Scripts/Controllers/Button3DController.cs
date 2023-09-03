using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3DController : MonoBehaviour, IClickable
{
    public UnityEvent OnClickButton;
    public UnityEvent OnHoldButton;
    public UnityEvent OnRightClickButton;
    public UnityEvent OnCancelPress;

    public bool wasPressedDown = false;

    public void CancelPress()
    {
        OnCancelPress?.Invoke();
    }

    public void OnClickRight()
    {
        OnRightClickButton?.Invoke();
    }

    public void OnHold()
    {
        OnHoldButton?.Invoke();
    }

    public void OnPressDown()
    {
        wasPressedDown = true;
    }

    public void OnPressUp()
    {
        if (wasPressedDown)
        {
            OnClickButton?.Invoke();
            wasPressedDown = false;
        }
    }
}
