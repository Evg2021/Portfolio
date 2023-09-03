using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleIntController : ControllerBase, IMultiTypesController
{
    protected ITrenInteractableInt currentTrenObjectSet;
    //TODO: Должен быть Int
    protected ITrenInteractableBool currentTrenObjectGet;

    protected int currentValue;

    public override Types GetControllerType()
    {
        return Types.TYPE_INT;
    }
    public override int GetTypesCount()
    {
        return 2;
    }

    public override dynamic GetSimulatorValue()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetSimulatorValue();
        }

        return null;
    }

    public override string GetTrenName()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetTrenName();
        }

        return null;
    }

    public override uint GetTrenObjectIndex()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetTrenIndex();
        }

        return uint.MaxValue;
    }

    public override void Initialize()
    {
        if (InitializeTrenObject(out currentTrenObjectSet, out currentTrenObjectGet))
        {
            Enable();

            currentValue = currentTrenObjectSet.GetSimulatorValue();
        }
        else
        {
            Disable();
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        currentValue += (int)value;

        if (withSimulator && currentTrenObjectSet != null)
        {
            currentTrenObjectSet.Interact(currentValue);
        }
    }

    public Types GetControllerTypeSet()
    {
        return GetControllerType();
    }

    public Types GetControllerTypeGet()
    {
        return Types.TYPE_BOOL;
    }
}
