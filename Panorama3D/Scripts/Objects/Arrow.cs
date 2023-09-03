using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrow : MovableObject
{
    public bool AllowEdit = true;

    public float MaxDistance = 150.0f;
    public float MinDistance = 10.0f;

    public Vector3 LocalPosition
    {
        get
        {
            if (child != null)
                return child.localPosition;
            else
            {
                Debug.LogError("Child object of arrow is missing.");
                return Vector3.zero;
            }
        }
        set
        {
            if(child != null)
            {
                child.localPosition = value;
            }
        }
    }

    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            transform.rotation = value;
        }
    }

    public override Vector3 PositionToCheckBorders
    {
        get
        {
            if (child != null)
                return child.position;
            else
            {
                Debug.LogError("Child object of arrow is missing.");
                return Vector3.zero;
            }
        }
    }

    public Vector3 NextPanoramaEulerAngles { get; private set; }
    public Panorama NextPanorama { get; private set; }
    public ArrowType ArrowType { get; private set; }

    private Vector3 oldLocalPosition;
    private Quaternion oldRotation;

    protected UIController ui => UIController.Instance;
    private Transform child;

    protected bool canMove;

    private float keyboardSpeed = 0.5f;

    protected override void Awake()
    {
        base.Awake();

        child = transform.GetChild(0);
        if (child == null)
            Debug.LogError("Arrow object have no child object with arrow model.");
    }

    protected override void Cancel()
    {
        if (gameObject != null && isHighlighted)
        {
            LocalPosition = oldLocalPosition;
            Rotation = oldRotation;
        }

        base.Cancel();
    }

    public override void CancelPress()
    {
        base.CancelPress();

        if (!cameraController.IsActive)
            cameraController.EnableControl();
    }

    public override void OnHold()
    {
        if (!AllowEdit) return;

        base.OnHold();
    }

    public override void OnPressDown()
    {
        base.OnPressDown();

        if (cameraController.IsActive)
            cameraController.DisableControl();

        oldLocalPosition = LocalPosition;
        oldRotation = Rotation;
    }

    public override void OnPressUp()
    {
        base.OnPressUp();

        if (!cameraController.IsActive)
            cameraController.EnableControl();

        if (NextPanorama != null && !ui.AdapterController.gameObject.activeSelf && timer > 0)
        {
            cameraController.SetRotation(NextPanoramaEulerAngles);
            PanoramaController.Instance.ChangePanorama(NextPanorama);

            //MapController.Instance.ShowLevel();
        }
    }

    public void SetNextPanorama(Panorama panorama, Vector3 nextEulerAngles)
    {
        NextPanorama = panorama;
        NextPanoramaEulerAngles = nextEulerAngles;
    }

    public void SetType(ArrowType type)
    {
        ArrowType = type;
    }

    public void AllowMove()
    {
        canMove = true;

        ui.CurrentArrow = gameObject;

        startCursor = input.CursorPosition() / Screen.dpi;
        cameraController.DisableControl();
    }

    private void Move()
    {
        if (input != null)
        {
            var keyboard = (InputKeyboard)input;
            if(keyboard != null)
            {
                var axises = keyboard.GetArrowsInput() * keyboardSpeed;

                transform.eulerAngles += new Vector3(0.0f, axises.x, 0.0f);
                LocalPosition         += new Vector3(0.0f, 0.0f,     axises.y);

                LocalPosition = new Vector3(LocalPosition.x, LocalPosition.y, Mathf.Clamp(child.localPosition.z, MinDistance, MaxDistance));

                if (axises.sqrMagnitude > 0.0f)
                {
                    startCursor = input.CursorPosition() / Screen.dpi;
                }
                else
                {
                    if (startCursor != input.CursorPosition() / Screen.dpi)
                    {
                        MoveByCursor();
                    }
                }

            }
            else
            {
                MoveByCursor();
            }

            CheckArrowBorders();
        }
    }

    private void ActionAfterArrowApproving(string path, Vector3 nextEulerAngles, bool addReverseArrow)
    {
        PanoramaController.Instance.ApproveArrow(this, path, nextEulerAngles, addReverseArrow);
        ui.CurrentArrow = null;
        cameraController.EnableControl();
    }

    public void Approve(string image)
    {
        canMove = false;
        DisableHighlight();
        var adapter = ui.AdapterController;
        if (adapter != null)
        {
            if (!adapter.SetPanorama(image, ActionAfterArrowApproving))
            {
                ui.Cancel();
            }
            else
            {
                ui.OpenAdapterMenu(ArrowType == ArrowType.STAIRS);
            }
        }
        else
        {
            Debug.LogError("AdapterPanelController component on AdapterPanel object is missing.");
        }
    }

    public void Approve()
    {
        canMove = false;
        DisableHighlight();
        var adapter = ui.AdapterController;
        if (adapter != null)
        {
            if (!adapter.SetPanorama(ActionAfterArrowApproving))
            {
                ui.Cancel();
            }
            else
            {
                ui.OpenAdapterMenu(ArrowType == ArrowType.STAIRS);
            }
        }
        else
        {
            Debug.LogError("AdapterPanelController component on AdapterPanel object is missing.");
        }
    }

    private void Update()
    {
        if (canMove)
        {
            Move();
            EnableHighlight();

            if (input.GetFirstKey())
            {
                Approve();
            }
        }
    }

    protected override void MoveByCursor()
    {
        var direction = Camera.main.transform.InverseTransformPoint(Camera.main.ScreenPointToRay(input.CursorPosition()).direction);
        var angleX = Vector2.Angle(Vector2.up, new Vector2(direction.z, direction.y)) + Camera.main.transform.eulerAngles.x - 90.0f;
        var angleY = 90.0f - Vector2.Angle(Vector2.up, new Vector2(direction.z, direction.x));
        var rotation = Quaternion.Euler(Vector3.up * (cameraController.transform.eulerAngles.y + angleY));

        var height = Mathf.Abs(LocalPosition.y);
        var angleLimit = Mathf.Atan(height / MaxDistance) * Mathf.Rad2Deg;
        if (angleX >= angleLimit)
        {
            var distance = height / Mathf.Tan(angleX * Mathf.Deg2Rad);
            LocalPosition = new Vector3(LocalPosition.x, LocalPosition.y, distance);
        }

        transform.rotation = rotation;
    }

    public override void OnClickRight()
    {
        if (!AllowEdit) return;

        if (!canMove && !ui.AdapterController.gameObject.activeSelf)
        {
            ui.AdapterController.ChangePanorama(NextPanorama.ImageLink, ActionAfterArrowApproving);
            ui.AdapterController.SetViewRotation(NextPanoramaEulerAngles);
            ui.OpenAdapterMenu(ArrowType == ArrowType.STAIRS);
            ui.CurrentArrow = gameObject;
            ui.AdapterController.SetToogleAutoReverseArrow(false);
        }
    }
}
