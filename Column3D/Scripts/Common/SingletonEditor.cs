using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SingletonEditor<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return FindObjectOfType<T>();
        }
    }

    #region Reset
#if UNITY_EDITOR
    private void Reset()
    {
        if (FindObjectsOfType<T>().Length > 1)
        {
            EditorUtility.DisplayDialog(GetType().Name + " is exist", "Scene have one instance of " + GetType().Name, "OK");
            DestroyImmediate(this);
        }
    }
#endif
    #endregion
}
