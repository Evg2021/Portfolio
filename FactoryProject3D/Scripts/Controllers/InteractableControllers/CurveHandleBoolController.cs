using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveHandleBoolController : DamperBoolController
{
    protected override string currentPositivePostfix { get { return "Отцепить клапан"; } }
    protected override string currentNegativePostfix { get { return "Зацепить клапан"; } }

    private void Reset()
    {
        RotationAngle = 45.0f;
        RotationAxis = Vector3.up;
    }
}
