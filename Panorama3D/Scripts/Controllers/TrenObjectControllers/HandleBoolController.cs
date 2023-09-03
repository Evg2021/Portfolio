using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBoolController : TrenObjectControllerBase
{
    public bool currentValue = false;
    unsafe private bool* getBool;
    unsafe private bool* setBool;

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);

        getBool = (bool*)get;
        setBool = (bool*)set;

        ClientSocketManager.UpdateGetOne(Index);
        currentValue = *getBool;

        InputBase.Instance.FirstKeyIsDown += Instance_FirstKeyIsDown;
    }

    private unsafe void Instance_FirstKeyIsDown(Vector2 obj)
    {
        if (isActive)
        {
            *setBool = !*getBool;
            ClientSocketManager.UpdateSetOne(Index);
        }
    }

    private void Update()
    {
        ClientSocketManager.UpdateGetOne(Index);
        unsafe
        {
            if (currentValue != *getBool)
            {
                currentValue = *getBool;
                OnChangedValue();
            }
        }
    }

    protected virtual void OnChangedValue()
    {
        Debug.Log($"Value Changed: {currentValue}.");
    }
}
