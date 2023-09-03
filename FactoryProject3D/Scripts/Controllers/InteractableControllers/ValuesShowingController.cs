using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValuesShowingController : ControllerBase
{
    private ITrenInteractableFloat currentTrenObjectSet;
    private ITrenInteractableFloat currentTrenObjectGet;

    public override Types GetControllerType()
    {
        return Types.TYPE_FLOAT;
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

    public override void Initialize()
    {
        if (InitializeTrenObject(out currentTrenObjectSet, out currentTrenObjectGet))
        {
            Enable();
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        
    }

    public override uint GetTrenObjectIndex()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetTrenIndex();
        }

        return uint.MaxValue;
    }
}
