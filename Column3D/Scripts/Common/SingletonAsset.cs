using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAsset<T> : ScriptableObject
    where T : ScriptableObject
{
    public static T Instance
    {
        get
        {
            Type type = typeof(T);
            return Resources.Load(type.Name, type) as T;
        }
    }
}
