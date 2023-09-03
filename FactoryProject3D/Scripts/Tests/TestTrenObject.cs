using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrenObject : MonoBehaviour
{
    public GameObject testInterafceObject;
    public TrenObject testObject;
    public TrenBool testBool;

    private ITrenInteractableBool testInterface;

    private void OnValidate()
    {
        if (testInterafceObject)
        {
            testInterface = testInterafceObject.GetComponent<ITrenInteractableBool>();

            if (testInterface != null && testObject && testBool)
            {
                InitializeTests();
            }
        }
    }

    private void InitializeTests()
    {
        testBool.Initialize("testName", "testParam", ObjectMode.SET);
        Debug.Log($"{testInterface.GetTrenName()} - interface name.");
        Debug.Log($"{testObject.GetObjectMode()} - object Objectmode");
    }
}
