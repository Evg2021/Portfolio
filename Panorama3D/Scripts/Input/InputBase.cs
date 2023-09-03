using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBase : SingletonMonoBehaviour<InputBase>
{
    public abstract event Action<Vector2> FirstKeyIsDown;
    public abstract event Action<Vector2> SecondKeyIsDown;
    public abstract event Action<Vector2> FirstKeyIsUp;
    public abstract event Action<Vector2> SecondKeyIsUp;
    public abstract event Action<Vector2> FirstKeyIsHold;

    public abstract bool GetFirstKey();

    public abstract bool GetSecondKey();

    public abstract Vector2 DiffCursorPosition();
    public abstract Vector2 CursorPosition();
    public abstract float ScrollDelta();

    public abstract void Refresh();
}
