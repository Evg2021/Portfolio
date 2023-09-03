using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntHeaderController : MonoBehaviour
{
    public string[] Headers;

    private TextMeshProUGUI header;
    private ControllerBase controller;
    private int currentValue;

    private void Awake()
    {
        InitializeHeader();
        InitializeController();
    }

    private void InitializeController()
    {
        controller = transform.parent.GetComponentInChildren<ControllerBase>();
        if (controller && controller.isEnabled && controller.GetControllerType() == Types.TYPE_INT)
        {
            SetIndex(controller.GetSimulatorValue());
        }
        else
        {
            currentValue = -1;
        }
    }
    private void InitializeHeader()
    {
        header = GetComponent<TextMeshProUGUI>();
        if (header)
        {
            header.text = string.Empty;
        }
    }

    private void Update()
    {
        if (controller && header && controller.isEnabled && controller.GetControllerType() == Types.TYPE_INT)
        {
            if (currentValue != controller.GetSimulatorValue())
            {
                SetIndex(controller.GetSimulatorValue());
            }
        }
    }

    private void SetIndex(int index)
    {
        currentValue = index;
        if (header && Headers != null && Headers.Length > index && index >= 0)
        {
            header.text = Headers[index];
        }
    }
}