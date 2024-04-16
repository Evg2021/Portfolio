using System;
using UnityEngine;

public class InteractiveLight : Interactive
{
    [SerializeField] private Light _light;

    public override void OnInteractStart(object value)
    {
        _light.enabled = Convert.ToBoolean(value);
    }
}
