using System;
using System.Collections.Generic;
using UnityEngine;

public class HologramInterract : Interactable
{
    [SerializeField] private RaycastEntity _hiddenObject;
    public RaycastEntity HiddenObject => _hiddenObject;

    public bool IsPlaced { get; private set; } = false;
    private void Awake()
    {
        _hiddenObject.gameObject.SetActive(false);
    }

    public void Change()
    {
        IsPlaced = true;
        _hiddenObject.gameObject.SetActive(true);
        _hiddenObject.transform.parent = null;
        gameObject.SetActive(false);
    }
    // public List<MeshRenderer> _solidColors;
    // public List<MeshRenderer> _SelfMeshRenderers;
    // public bool IsPlace { get; private set; }
    // public void Change()
    // {
    //     for (int i = 0; i < _solidColors.Count; i++)
    //     {
    //         _solidColors[i].enabled = true;
    //         _SelfMeshRenderers[i].enabled = false;
    //     }
    //     
    //     IsPlace = true;
    // }
}