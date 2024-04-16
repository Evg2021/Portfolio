using System.Collections.Generic;
using UnityEngine;

public class StatickChange : MonoBehaviour
{
    public StatickObjHolder StatickObjHolder;

    [ContextMenu("Disable and save")]
    private void DisableStatic()
    {
        if (StatickObjHolder.StaticObjects.Count > 0)
        {
            print("Static's array not empty!!!");
            DisplayCountInStatic();
            return;
        }
        
        List<Transform> _childs = new();
        _childs.AddRange(transform.GetComponentsInChildren<Transform>());

        foreach (var child in _childs)
        {
            if (child.gameObject.isStatic)
            {
                StatickObjHolder.StaticObjects.Add(child);
                child.gameObject.isStatic = false;
            }
        }

        _childs.Clear();
        DisplayCountInStatic();
    }

    [ContextMenu("Enable")]
    private void RevertStatic()
    {
        foreach (var child in StatickObjHolder.StaticObjects)
        {
            if (child == null)
            {
                print("One of objects are missing!");
                continue;
            }
            
            child.gameObject.isStatic = true;
        }
        
        print(StatickObjHolder.StaticObjects.Count + " objects are static again");
        ClearStaticArray();
    }

    [ContextMenu("Display Count In Static")]
    private void DisplayCountInStatic()
    {
        print("Static's array has a " + StatickObjHolder.StaticObjects.Count + " obj");
    }

    [ContextMenu("Clear Static Array")]
    private void ClearStaticArray()
    {
        StatickObjHolder.StaticObjects.Clear();
        print("Static's array has been clear");
    }
}