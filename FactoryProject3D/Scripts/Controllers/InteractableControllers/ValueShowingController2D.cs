using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueShowingController2D : ControllerBase
{
    private ITrenInteractableFloat currentTrenObject;
    private TextMeshProUGUI header;

    private void Awake()
    {
        header = GetComponent<TextMeshProUGUI>();
        if (header)
        {
            header.text = string.Empty;
        }
    }

    private void Update()
    {
        if (header && currentTrenObject != null && currentTrenObject.IsObjectRegistrated())
        {
            header.text = currentTrenObject.GetSimulatorValue().ToString();
        }
    }

    public override Types GetControllerType()
    {
        return Types.TYPE_FLOAT;
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
        if (InitializeTrenObject<ITrenInteractableFloat, ITrenInteractableFloat>(out currentTrenObject, out _))
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
        
    }
}
