using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetComponent : MonoBehaviour
{
    public GameObject test;
    public int testCount = 1000;

    private void Start()
    {
        float startTime = Time.realtimeSinceStartup;

        if (test)
        {
            for (int i = 0; i < testCount; i++)
            {
                test.GetComponent<ControllerBase>();
                test.GetComponent<ControllerBase>();
            }
        }

        float endTime = Time.realtimeSinceStartup;

        Debug.Log("Time: " + (endTime - startTime));
    }
}
