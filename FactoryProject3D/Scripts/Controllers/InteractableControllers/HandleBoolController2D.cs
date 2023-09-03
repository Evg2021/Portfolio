using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleBoolController2D : ControllerBase, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnClickButtonDown;
    public UnityEvent OnClickButtonUp;

    private ITrenInteractableBool currentTrenObject;

    private bool isButtonHeldDown;

    private void Update()
    {
        if (isButtonHeldDown)
        {
            Interact();
        }
    }

    public override Types GetControllerType()
    {
        return Types.TYPE_BOOL;
    }

    public override dynamic GetSimulatorValue()
    {
        if (currentTrenObject != null)
        {
            return currentTrenObject.GetSimulatorValue();
        }

        return null;
    }

    public override string GetTrenName()
    {
        if (currentTrenObject != null)
        {
            return currentTrenObject.GetTrenName();
        }

        return null;
    }

    public override uint GetTrenObjectIndex()
    {
        if (currentTrenObject != null)
        {
            return currentTrenObject.GetTrenIndex();
        }

        return uint.MaxValue;
    }

    public override void Initialize()
    {
        if (InitializeTrenObject<ITrenInteractableBool, ITrenInteractableBool>(out currentTrenObject, out _))
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    public override void Interact(dynamic value = null, bool withSimulator = true)
    {
        if (currentTrenObject != null)
        {
            currentTrenObject.Interact(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isEnabled)
        {
            isButtonHeldDown = false;
            OnClickButtonUp?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEnabled)
        {
            isButtonHeldDown = true;
            OnClickButtonDown?.Invoke();
        }
    }
}
