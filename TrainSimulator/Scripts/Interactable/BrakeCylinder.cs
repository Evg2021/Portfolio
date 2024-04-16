using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeCylinder : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    private void OnEnable()
    {
        _model.SetActive(true);
    }
    
    private void OnDisable()
    {
        _model.SetActive(false);
    }
}
