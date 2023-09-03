using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShowingController : ControllerBase
{
    private const string emissionShaderKeyname = "_EMISSION";

    private ITrenInteractableBool currentTrenObjectGet;
    private ITrenInteractableBool currentTrenObjectSet;

    private bool currentValue;
    private Material currentMaterial;

    public override Types GetControllerType()
    {
        return Types.TYPE_BOOL;
    }

    public override dynamic GetSimulatorValue()
    {
        return null;
    }

    public override string GetTrenName()
    {
        return null;
    }

    public override uint GetTrenObjectIndex()
    {
        return currentTrenObjectGet.GetTrenIndex();
    }

    private void Update()
    {
        if (isEnabled && currentTrenObjectGet != null)
        {
            if (currentValue != currentTrenObjectGet.GetSimulatorValue())
            {
                currentValue = currentTrenObjectGet.GetSimulatorValue();
                SetLight(currentValue);
            }
        }
    }

    public override void Initialize()
    {
        if (InitializeTrenObject(out currentTrenObjectGet, out currentTrenObjectSet))
        {
            Enable();
            if (TryGetComponent<MeshRenderer>(out var renderer))
            {
                currentMaterial = renderer.material;
            }
        }
        else
        {
            Disable();
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        
    }

    private void SetLight(bool value)
    {
        if (currentMaterial)
        {
            if (value)
            {
                currentMaterial.EnableKeyword(emissionShaderKeyname);
            }
            else
            {
                currentMaterial.DisableKeyword(emissionShaderKeyname);
            }
        }
    }
}
