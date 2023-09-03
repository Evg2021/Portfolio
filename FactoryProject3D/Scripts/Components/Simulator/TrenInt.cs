using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenInt : TrenObject, ITrenInteractableInt
{
    private int setParameter;
    private int getParameter;

    public int GetLocalValue()
    {
        return setParameter;
    }

    public int GetSimulatorValue()
    {
        return getParameter;
    }

    public void Interact(int value)
    {
        ChangeSetter(value);
        UpdateSetter();
    }

    public override void RegistrateObject()
    {
        unsafe
        {
            fixed (int* get = &getParameter, set = &setParameter)
            {
                Index = ClientSocketManager.RegistrateParameter(TrenName, TrenParameter, Types.TYPE_INT, get, Types.TYPE_INT, set);

                if (Index != uint.MaxValue)
                {
                    setParameter = getParameter;
                    IsRegistrated = true;
                }
            }
        }
    }

    public void ChangeSetter(int value)
    {
        setParameter = value;
    }
}
