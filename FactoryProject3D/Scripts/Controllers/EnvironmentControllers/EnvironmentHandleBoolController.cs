using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentHandleBoolController : EnvironmentControllerBase
{
    public UnityEvent OnPressButton;
    public UnityEvent OnSecondPressButton;

    public override void Interact()
    {
        CurrentValue = !CurrentValue;
        if (CurrentValue)
        {
            OnPressButton?.Invoke();
        }
        else
        {
            OnSecondPressButton?.Invoke();
        }
    }

    public override void SetState(bool state)
    {
        CurrentValue = state;
        if (CurrentValue)
        {
            OnPressButton?.Invoke();
        }
        else
        {
            OnSecondPressButton?.Invoke();
        }
    }
}
