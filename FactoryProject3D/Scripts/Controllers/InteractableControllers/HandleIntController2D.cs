using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleIntController2D : ControllerBase
{
    private ITrenInteractableInt currentTrenObject;

    public override Types GetControllerType()
    {
        return Types.TYPE_INT;
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
        if (InitializeTrenObject<ITrenInteractableInt, ITrenInteractableInt>(out currentTrenObject, out _))
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
        if (currentTrenObject != null && value != null)
        {
            currentTrenObject.Interact(currentTrenObject.GetSimulatorValue() + value);

        }
    }

    public void Increase()
    {
        Interact(1);
    }
    public void Decrease()
    {
        Interact(-1);
    }
}
