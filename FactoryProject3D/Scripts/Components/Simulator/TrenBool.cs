using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenBool : TrenObject, ITrenInteractableBool
{
    private bool setParameter;
    private bool getParameter;

    public bool GetLocalValue()
    {
        return setParameter;
    }

    public bool GetSimulatorValue()
    {
        return getParameter;
    }

    public void Interact(bool value)
    {
        ChangeSetter(value);
        UpdateSetter();
    }

    public override void RegistrateObject()
    {
        unsafe
        {
            fixed (bool* get = &getParameter, set = &setParameter)
            {
                Index = ClientSocketManager.RegistrateParameter(TrenName, TrenParameter, Types.TYPE_BOOL, get, Types.TYPE_BOOL, set);

                if (Index != uint.MaxValue)
                {
                    setParameter = getParameter;
                    IsRegistrated = true;
                }
            }
        }
    }

    public void ChangeSetter(bool value)
    {
        setParameter = value;
    }
}
