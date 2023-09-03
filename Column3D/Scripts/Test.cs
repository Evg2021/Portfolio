using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    public ScenarioAsset testAsset;

    private void OnValidate()
    {
        if (testAsset != null)
        {
            var test = testAsset.Stages[94].DescriptionsInfo[0].Message;
            Debug.Log(test);
        }
    }
}
