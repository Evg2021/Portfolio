using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestValve : MonoBehaviour
{
    public GameObject test;
    public TrenObject[] testObjects;

    private void OnValidate()
    {
        //testObjects = FindObjectsOfType<TrenObject>();
        //gameObject.AddComponent<HandleBoolController>();
    }

    private void Start()
    {
        var time = Time.realtimeSinceStartup;
        float x = 0;
        for (int i = 0; i < 10000; i++)
        {
            test.GetComponentInChildren<TrenFloat>();
            test.GetComponent<TrenFloat>();
            test.GetComponentInParent<TrenFloat>();
        }

        Debug.Log($"X = {x}");
        Debug.Log($"Time : {Time.realtimeSinceStartup - time:0.00000}");
    }

}
