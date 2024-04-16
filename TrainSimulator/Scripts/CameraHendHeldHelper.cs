using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHendHeldHelper : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _cameraHH;

    private void Update()
    {
        if (_cameraHH.fieldOfView != _camera.fieldOfView)
            _cameraHH.fieldOfView = _camera.fieldOfView;
    }
}