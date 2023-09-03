using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBoolController : ControllerBase, ICustomInfoController
{
    private const string radarName = "Radar";

    protected ITrenInteractableBool currentTrenObjectGet;
    protected ITrenInteractableBool currentTrenObjectSet;

    protected virtual string currentPositivePostfix { get { return "Отцепить клапан";}}
    protected virtual string currentNegativePostfix { get { return "Зацепить клапан";}}

    protected bool stateCheckingAllowed = false;

    public override Types GetControllerType()
    {
        return Types.TYPE_BOOL;
    }

    public virtual string GetPostfix()
    {
        if (currentTrenObjectGet != null)
        {
            if (currentTrenObjectGet.GetSimulatorValue())
            {
                return currentPositivePostfix;
            }
            else
            {
                return currentNegativePostfix;
            }
        }

        return string.Empty;
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

        return string.Empty;
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
        }
        else
        {
            Disable();
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        if (value == null && currentTrenObjectGet != null)
        {
            value = !currentTrenObjectGet.GetSimulatorValue();
        }

        if (withSimulator && currentTrenObjectSet != null)
        {
            currentTrenObjectSet.Interact(value);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!stateCheckingAllowed && other.name == radarName)
        {
            stateCheckingAllowed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (stateCheckingAllowed && other.name == radarName)
        {
            stateCheckingAllowed = false;
        }
    }

    public override int GetTypesCount()
    {
        return 1;
    }
}
