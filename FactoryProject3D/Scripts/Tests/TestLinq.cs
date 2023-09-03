using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestLinq : MonoBehaviour
{
    public List<string> test;
    public string[] result;

    private void OnValidate()
    {
        result = test.Where(h => h.Contains("test")).ToArray();
    }
}
