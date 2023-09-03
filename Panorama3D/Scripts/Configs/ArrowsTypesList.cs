using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ArrowsList", menuName = "Create Arrows list", order = 0)]
public class ArrowsTypesList : ScriptableObject
{
    public List<ArrowObject> ArrowObjects;

    public GameObject GetArrow(ArrowType type)
    {
        if(ArrowObjects != null && ArrowObjects.Count > 0)
        {
            return ArrowObjects.FirstOrDefault(h => h.ArrowType == type).ArrowGameObject;
        }

        return null;
    }

    public GameObject GetArrow(int intType)
    {
        var type = (ArrowType)intType;

        if (ArrowObjects != null && ArrowObjects.Count > 0)
        {
            return ArrowObjects.FirstOrDefault(h => h.ArrowType == type).ArrowGameObject;
        }

        return null;
    }
}

public enum ArrowType
{
    ARROW, STAIRS
}

[Serializable]
public struct ArrowObject
{
    public ArrowType ArrowType;
    public GameObject ArrowGameObject;
}

