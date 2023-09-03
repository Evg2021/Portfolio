using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodController : MonoBehaviour
{
    [SerializeField]
    private float maxOffsetZ = -0.0649613f;
    private float startPositionZ;

    private ValveController valve;

    private void Awake()
    {
        startPositionZ = transform.localPosition.z;
        if (transform.parent != null)
        {
            valve = transform.parent.GetComponentInChildren<ValveController>();
        }
    }

    private void Update()
    {
        if (valve)
        {
            var currentValue = valve.GetSimulatorValue();
            if(currentValue != null && currentValue.GetType() == typeof(float))
            transform.localPosition = new Vector3(transform.localPosition.x,
                                                  transform.localPosition.y,
                                                  Mathf.Lerp(startPositionZ + maxOffsetZ, startPositionZ, valve.GetSimulatorValue() / 100));
        }
    }
}
