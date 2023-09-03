using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesController : MonoBehaviour
{
    public void MakePlatesNonClickable()
    {
        SetPlatesClickableState(false);
    }
    public void MakePlatesClickable()
    {
        SetPlatesClickableState(true);
    }
    private void SetPlatesClickableState(bool value)
    {
        var keyObjects = GetComponentsInChildren<KeyObject>();

        foreach (var keyObject in keyObjects)
        {
            keyObject.SetClickable(value);
        }
    }
}
