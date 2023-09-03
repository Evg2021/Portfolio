using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchHIntController : HandleIntController, ICustomInfoController
{
    protected string currentPositivePostfix { get { return "Выключить насос"; } }
    protected string currentNegativePostfix { get { return "Включить насос"; } }

    [SerializeField]
    private float angleStep = 45;

    [SerializeField]
    private Vector3 axisRotation = Vector3.up;

    [SerializeField]
    private int maxValue = 2;

    private Quaternion originRotation;

    public string GetPostfix()
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

    public override void Initialize()
    {
        base.Initialize();
        InitializeRotations();
    }

    private void InitializeRotations()
    {
        originRotation = transform.localRotation * Quaternion.Euler(axisRotation * angleStep);
        transform.localRotation = originRotation * Quaternion.Euler(axisRotation * angleStep * currentValue);
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        int newValue = currentValue + (int)value;

        newValue = Mathf.Clamp(newValue, 0, maxValue);

        if (withSimulator && currentTrenObjectSet != null)
        {
            currentTrenObjectSet.Interact(newValue);
        }
    }

    private void Update()
    {
        if (currentTrenObjectSet != null)
        {
            if (currentValue != currentTrenObjectSet.GetSimulatorValue())
            {
                currentValue = currentTrenObjectSet.GetSimulatorValue();
                transform.localRotation = originRotation * Quaternion.Euler(axisRotation * angleStep * currentValue);
            }
            //TODO: Временно
            if (currentValue == 2)
            {
                currentTrenObjectSet.Interact(1);
            }
        }
    }
}
