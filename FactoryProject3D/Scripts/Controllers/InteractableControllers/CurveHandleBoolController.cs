using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveHandleBoolController : DamperBoolController
{
    protected override string currentPositivePostfix { get { return "�������� ������"; } }
    protected override string currentNegativePostfix { get { return "�������� ������"; } }

    private void Reset()
    {
        RotationAngle = 45.0f;
        RotationAxis = Vector3.up;
    }
}
