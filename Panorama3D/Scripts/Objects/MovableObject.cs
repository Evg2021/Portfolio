using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour, IClickable
{
    [SerializeField]
    protected float TimeToHoldForMove = 0.25f;

    [SerializeField]
    private Material HighlightMaterial;

    public virtual Vector3 PositionToCheckBorders
    {
        get
        {
            return transform.position;
        }
    }

    protected new MeshRenderer renderer;
    protected Material ownMaterial;
    private float fixedPositionZ;

    protected InputBase input;
    protected CameraController cameraController;

    protected Vector2 startCursor;
    protected float timer;
    protected bool isHighlighted;
    protected bool allowToChangePosition;

    public virtual void CancelPress()
    {
        DisableHighlight();
    }

    protected virtual void Cancel()
    {
        DisableHighlight();
    }

    public virtual void OnClickRight()
    {
        Debug.Log("Object " + transform.name + " has right click. Nonimplemented function.");
    }

    public virtual void OnHold()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0 && startCursor == input.CursorPosition() / Screen.dpi)
        {
            EnableHighlight();
        }
        else if (timer <= 0 && startCursor != input.CursorPosition() / Screen.dpi)
        {
            timer = TimeToHoldForMove;
            startCursor = input.CursorPosition() / Screen.dpi;
        }

        if (allowToChangePosition)
        {
            if (cameraController.IsActive)
                cameraController.DisableControl();

            MoveByCursor();
            CheckArrowBorders();
        }
    }

    public virtual void OnPressDown()
    {
        timer = TimeToHoldForMove;
        startCursor = input.CursorPosition() / Screen.dpi;
        fixedPositionZ = Camera.main.transform.InverseTransformPoint(transform.position).z;
    }

    public virtual void OnPressUp()
    {
        DisableHighlight();
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if(GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
        {
            Debug.LogError("Collider component is missing on " + transform.name + " object.");
        }

        input = InputBase.Instance;
        if (input == null)
            Debug.LogError("Input component is missing on scene.");

        cameraController = CameraController.Instance;
        if (cameraController == null)
            Debug.LogError("CameraController component was not found.");

        if (transform.childCount > 0)
        {
            renderer = transform.GetComponentInChildren<MeshRenderer>();
        }
        else
        {
            renderer = GetComponent<MeshRenderer>();
        }
        if (renderer != null)
        {
            ownMaterial = renderer.sharedMaterial;
        }
        else
        {
            Debug.LogError("MeshRenderer on " + transform.name + " is missing.");
        }

        startCursor = input.CursorPosition() / Screen.dpi;

        InitializeKeyboardEvents();
    }

    protected virtual void OnDisable()
    {
        RemoveEvents();
    }

    protected virtual void OnDestroy()
    {
        RemoveEvents();
    }

    protected virtual void InitializeKeyboardEvents()
    {
        var keyboard = (InputKeyboard)input;
        if (keyboard != null)
        {
            keyboard.HotKeyCancel += Cancel;
        }
    }

    protected virtual void RemoveEvents()
    {
        var keyboard = (InputKeyboard)input;
        if (keyboard != null)
        {
            keyboard.HotKeyCancel -= Cancel;
        }
    }

    protected void DisableHighlight()
    {
        allowToChangePosition = false;

        if (isHighlighted && renderer != null && ownMaterial != null)
        {
            renderer.sharedMaterial = ownMaterial;
            isHighlighted = false;
        }
    }

    protected void EnableHighlight()
    {
        allowToChangePosition = true;

        if (!isHighlighted && renderer != null && HighlightMaterial != null)
        {
            renderer.sharedMaterial = HighlightMaterial;
            isHighlighted = true;
        }
    }

    protected virtual void MoveByCursor()
    {
        var cursorPosition = input.CursorPosition();
        var newPosition = Camera.main.ScreenToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, fixedPositionZ));
        transform.position = newPosition;
    }

    protected void CheckArrowBorders()
    {
        var ownAngle = cameraController.GetRelativeAngleY(PositionToCheckBorders);
        var borderAngle = cameraController.HorizontalFOV * 0.5f;

        if (ownAngle >= borderAngle)
        {
            var k = (Camera.main.WorldToScreenPoint(PositionToCheckBorders).x - Screen.width * 0.5f >= 0) ? 1.0f : -1.0f;
            cameraController.RotateY((ownAngle - borderAngle) * k);
        }
    }
}
