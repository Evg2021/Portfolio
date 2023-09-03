using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParameter
{
    public abstract bool IsParameterChanged();
    public abstract void Apply();
}
