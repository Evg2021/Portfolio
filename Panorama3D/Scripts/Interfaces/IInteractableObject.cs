using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    public Vector3 GetPosition();
    public Quaternion GetRotation();
    public void Remove();
    public bool Approved();
    public void SetClickable(bool value);
}
