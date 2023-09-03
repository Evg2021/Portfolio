using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    public void OnPressUp();
    public void OnPressDown();
    public void OnHold();
    public void CancelPress();
    public void OnClickRight();
}
