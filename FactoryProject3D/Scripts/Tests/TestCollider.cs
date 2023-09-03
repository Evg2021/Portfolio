using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    public ValveController test;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
