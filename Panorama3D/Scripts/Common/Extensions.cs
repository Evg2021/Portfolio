using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector2 GetXY(this Vector3 position)
    {
        return new Vector2(position.x, position.y);
    }

    public static List<Vector3> GetListValues(this Dictionary<string, Vector3> dictionary)
    {
        var result = new List<Vector3>();

        foreach(var item in dictionary)
        {
            result.Add(item.Value);
        }

        return result;
    }

    public static bool ChangeKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey fromKey, TKey toKey)
    {
        if (dictionary.ContainsKey(fromKey))
        {
            TValue value = dictionary[fromKey];
            dictionary.Remove(fromKey);
            dictionary.Add(toKey, value);

            return true;
        }

        return false;
    }
}
