using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamperBoolController : HandleBoolController, ICustomInfoController
{
    protected override string currentPositivePostfix { get { return "Закрыть"; } }
    protected override string currentNegativePostfix { get { return "Открыть"; } }

    public float RotationAngle = 180.0f;
    public Vector3 RotationAxis = Vector3.forward;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private bool currentValue = false;

    public override void Initialize()
    {
        if (InitializeTrenObject(out currentTrenObjectSet, out currentTrenObjectGet))
        {
            InitializeRotations();
            currentValue = currentTrenObjectGet.GetSimulatorValue();
            SetMeshState();
            Enable();
        }
        else
        {
            Disable();
        }
    }
    private void InitializeRotations()
    {
        closedRotation = transform.localRotation;
        Vector3 closedRotationEulerAngles = closedRotation.eulerAngles;
        openedRotation = Quaternion.Euler(closedRotationEulerAngles + (RotationAxis * RotationAngle));
    }

    private void Update()
    {
        if (stateCheckingAllowed && currentTrenObjectGet != null)
        {
            if (currentTrenObjectGet.GetSimulatorValue() != currentValue)
            {
                currentValue = currentTrenObjectGet.GetSimulatorValue();
                SetMeshState();
            }
        }
    }

    private void SetMeshState()
    {
        if (currentValue)
        {
            RotateToOpenMesh();
        }
        else
        {
            RotateToCloseMesh();
        }
    }
    private void RotateToOpenMesh()
    {
        transform.localRotation = openedRotation;
    }
    private void RotateToCloseMesh()
    {
        transform.localRotation = closedRotation;
    }
}
