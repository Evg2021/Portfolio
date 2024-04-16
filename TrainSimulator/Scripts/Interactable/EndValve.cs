using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndValve : RotatingSwitchDiscrete
{
    [SerializeField] private GameObject _steam;

    public override void EndInteract()
    {
        base.EndInteract();
        if (GetValue() == 1)
        {
            if(_steam != null)
            _steam.SetActive(false);
            else
            {
                Debug.LogError("Steam is no attached");
            }
        }
    }
}
