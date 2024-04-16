using System;
using UnityEngine;

public class DoorHidden : Door
{
    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = _door.position;
    }

    public override void Open()
    {
        _door.position = new Vector3(_startPos.x, -10, _startPos.z);
    }

    public override void CloseDoor()
    {
        _door.position = _startPos;
    }
}