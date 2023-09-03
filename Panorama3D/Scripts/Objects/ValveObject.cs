using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ValveObject : MovableObject, IInteractableObject, ITrenObject
{
    private const string unityNamePattern = "{0}\\{1}";

    [DllImport("PultControl.dll", EntryPoint = "OpenPult", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int fnOnOpenPult(string lpszPultName, string lpszWinddowName, int Type);

    [DllImport("PultControl.dll", EntryPoint = "ClosePult", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int fnOnClosePult(string lpszPultName);

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private float maxDistance = 150.0f;
    private float minDistance = 10.0f;
    private bool allowMoving = false;

    private string TrenObjectName;
    private string PanoramaName;
    private string PultTypeName;

    public void Initialize(TrenObjectInfo info = null)
    {
        allowMoving = true;
        cameraController.DisableControl();
        EnableHighlight();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    protected override void Cancel()
    {
        if(allowMoving)
        {
            Remove();
            return;
        }

        if (gameObject != null && isHighlighted)
        {
            transform.position = oldPosition;
            transform.rotation = oldRotation;
        }

        base.Cancel();
    }

    public override void CancelPress()
    {
        base.CancelPress();

        if (allowMoving)
            allowMoving = false;

        if (!cameraController.IsActive)
            cameraController.EnableControl();
    }

    public override void OnPressDown()
    {
        base.OnPressDown();

        if (cameraController.IsActive)
            cameraController.DisableControl();

        oldPosition = transform.position;
        oldRotation = transform.rotation;
    }

    public override void OnPressUp()
    {
        base.OnPressUp();

        if (!allowMoving)
        {
            UIController.Instance.ShowValveMenu(input.CursorPosition(), this);
        }

        if (allowMoving)
            allowMoving = false;

        if (!cameraController.IsActive)
            cameraController.EnableControl();
    }

    private void Update()
    {
        if (allowMoving)
        {
            MoveByCursor();
            CheckArrowBorders();

            if (input.GetFirstKey())
            {

            }
        }
    }

    protected override void MoveByCursor()
    {
        var mousePoistion = input.CursorPosition();
        var newPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePoistion.x, mousePoistion.y, maxDistance * 0.125f));

        transform.position = newPosition;

        transform.LookAt(Camera.main.transform);
    }

    public override void OnClickRight()
    {
        if (!allowMoving)
        {
            UIController.Instance.ShowObjectMenu(input.CursorPosition(), this);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public string GetTrenName()
    {
        return TrenObjectName;
    }

    public string GetTemplateName()
    {
        return PultTypeName;
    }

    public string GetPanoramName()
    {
        return PanoramaName;
    }

    public void SetTrenObjectName(string name)
    {
        throw new System.NotImplementedException();
    }

    public void SetPultObjectName(string name)
    {
        throw new System.NotImplementedException();
    }

    public void SetPanoramaName(string name)
    {
        throw new System.NotImplementedException();
    }

    public TrenObjectInfo GetTrenObjectInfo()
    {
        throw new System.NotImplementedException();
    }

    public bool Approved()
    {
        throw new System.NotImplementedException();
    }

    public void SetClickable(bool value)
    {
        throw new System.NotImplementedException();
    }
}

