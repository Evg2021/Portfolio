using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricHose : Interactable
{
    [SerializeField] private GameObject _ropedHose;
    public override void StartInteract()
    {
        base.StartInteract();
        if (CanInterract)
        {
            _ropedHose.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
