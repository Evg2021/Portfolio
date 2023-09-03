using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController
{
    public static bool IsActive
    {
        get
        {
            return !CameraController.isMoving;
        }
    }

}
