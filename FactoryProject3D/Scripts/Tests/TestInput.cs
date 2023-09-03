using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    public Vector2 move;

    public void OnMoveForward(InputValue value)
    {
        var y = value.Get<float>();
        move.y = y;
        Debug.Log("Y - " + y);
    }

    public void OnMoveSide(InputValue value)
    {
        var x = value.Get<float>();
        move.x = x;
        Debug.Log("X - " + x);
    }
}



