using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject
    where T : ScriptableObject
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T result = Resources.Load<T>(typeof(T).ToString());

                if (result == null)
                {
                    Debug.LogError($"Singleton ScriptableObject of {typeof(T)} type was not found in Resources directory.");
                    return null;
                }
                
                /*if (results.Length > 1)
                {
                    Debug.LogError($"Count of Singleton ScriptableObject of {typeof(T)} type is more than one.");
                    return null;
                }*/

                instance = result;
            }

            return instance;
        }
    }
}
