using System;
using UnityEngine;

[Serializable]
public class DistanceParameters
{
    public string DistanceUnit => _distanceUnit;
    [SerializeField] private string _distanceUnit;
    public float NearDistance => _nearDistance;
    [SerializeField][Min(0)] private float _nearDistance;

    public string GetDistanceString(Vector3 a, Vector3 b, bool returnEmptyIfNearDistance = false)
    {
        float distance = Vector3.Distance(a, b);

        if (returnEmptyIfNearDistance)
            if (distance < _nearDistance)
                return "";

        return distance.ToString("0.#") + _distanceUnit;
    }
}