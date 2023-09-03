using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveTemplateController : MonoBehaviour
{
    public float SpeedRotation = 50.0f;
    public Transform Valve;

    public void RotateIncrease()
    {
        if (Valve)
        {
            Valve.Rotate(Vector3.up, -SpeedRotation * Time.deltaTime);
        }
    }
    public void RotateDecrease()
    {
        if (Valve)
        {
            Valve.Rotate(Vector3.up, SpeedRotation * Time.deltaTime);
        }
    }
}
