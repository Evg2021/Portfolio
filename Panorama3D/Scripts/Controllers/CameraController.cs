using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : SingletonMonoBehaviour<CameraController>
{
    [Range(1.0f, 100.0f)]
    public float Speed = 10.0f;

    public float test;

    public bool IsActive
    {
        get
        {
            return allowedToUse;
        }
    }

    public float HorizontalFOV
    {
        get
        {
            var camera = GetComponent<Camera>();
            var ratio = Screen.width / Screen.height;

            return Mathf.Atan(ratio * Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
        }
    }

    private InputBase input;
    private Vector3 startRotation;
    private new Camera camera;

    private bool controlled;
    private bool allowedToUse;

    private float limitDiffPositionY = 0.0f;
    private bool blockY = false;

    private float maxVertAngle =  90.0f;
    private float minVertAngle = -90.0f;
    private float minFOV       =  5.0f;
    private float maxFOV       =  89.9f;

    private void Start()
    {
        input = InputBase.Instance;
        if (input == null)
            Debug.LogError("Input component is missing on scene.");

        camera = GetComponent<Camera>();

        controlled = true;
        EnableControl();
    }

    public void EnableControl()
    {
        input.Refresh();
        allowedToUse = true;
    }

    public void DisableControl()
    {
        allowedToUse = false;
    }

    public void SetRotation(Vector3 eulerAngles)
    {
        transform.localEulerAngles = eulerAngles;
        startRotation = transform.localEulerAngles;
    }

    public void RotateY(float angle)
    {
        transform.eulerAngles += Vector3.up * angle / Speed;
    }

    public float GetRelativeAngleY(Vector3 position)
    {
        var targetPosition = new Vector2(position.x, position.z);
        var ownPosition = new Vector2(transform.forward.x, transform.forward.z);

        float angle = Mathf.Acos(Vector2.Dot(targetPosition, ownPosition) / (targetPosition.magnitude * ownPosition.magnitude)) * Mathf.Rad2Deg;

        return angle;
    }

    private void RotateCamera()
    {
        if (allowedToUse)
        {
            if (controlled)
            {
                if (input.GetFirstKey())
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        var diffPosition = input.DiffCursorPosition() * Speed / Screen.dpi;

                        float diffY = diffPosition.y;

                        if(blockY)
                        {
                            if (Utilities.WrapAngle(transform.eulerAngles.x) >= maxVertAngle)
                                diffY = diffY < limitDiffPositionY ? diffY : limitDiffPositionY;
                            else
                                diffY = diffY >= limitDiffPositionY ? diffY : limitDiffPositionY;
                        }

                        var newEulerAngles = startRotation + new Vector3(diffY, -diffPosition.x, 0);
                        test = diffPosition.x;

                        var unclampedWrapedX = Utilities.WrapAngle(newEulerAngles.x);
                        var clampedUnwrapX = Utilities.UnwrapAngle(Mathf.Clamp(unclampedWrapedX, minVertAngle, maxVertAngle));
                        newEulerAngles.x = clampedUnwrapX;

                        if((unclampedWrapedX > maxVertAngle || unclampedWrapedX < minVertAngle) && !blockY)
                        {
                            blockY = true;
                            limitDiffPositionY = diffPosition.y;
                        }
                        else if (unclampedWrapedX <= maxVertAngle && unclampedWrapedX >= minVertAngle && blockY)
                        {
                            blockY = false;
                        }

                        transform.eulerAngles = newEulerAngles;
                    }
                    else
                    {
                        controlled = false;
                    }
                }
                else if (!input.GetFirstKey())
                {
                    startRotation = transform.eulerAngles;
                }
            }
            else
            {
                if (!input.GetFirstKey())
                    controlled = true;
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView - input.ScrollDelta(), minFOV, maxFOV);
            }
        }
    }

    private void Update()
    {
        RotateCamera();
    }

}
