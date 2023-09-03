using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchHBoolController : DamperBoolController
{
    protected override string currentPositivePostfix { get { return "Выключить"; } }
    protected override string currentNegativePostfix { get { return "Включить"; } }

    private void Reset()
    {
        RotationAngle = -90;
        RotationAxis = Vector3.right;
    }
}
