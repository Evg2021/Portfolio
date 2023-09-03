using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenFloat : TrenObject, ITrenInteractableFloat
{
    private float setParameter;
    private float getParameter;

    public void Interact(float value)
    {
        ChangeSetter(value);
        UpdateSetter();
    }

    public override void RegistrateObject()
    {
        unsafe
        {
            fixed (float* get = &getParameter, set = &setParameter)
            {
                Index = ClientSocketManager.RegistrateParameter(TrenName, TrenParameter, Types.TYPE_FLOAT, get, Types.TYPE_FLOAT, set);

                if (Index != uint.MaxValue)
                {
                    setParameter = getParameter;
                    IsRegistrated = true;
                }
            }
        }
    }

    public void ChangeSetter(float value)
    {
        setParameter += value;
    }

    public float GetSimulatorValue()
    {
        return getParameter;
    }

    public float GetLocalValue()
    {
        return setParameter;
    }
}
