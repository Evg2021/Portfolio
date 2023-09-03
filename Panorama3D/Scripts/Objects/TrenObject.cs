using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenObject : MovableObject, ITrenObject, IInteractableObject
{
    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private float maxDistance = 150.0f;
    private float minDistance = 10.0f;
    private bool allowMoving = false;

    private string trenObjectName;
    private string panoramName;
    private string templateTypeName;

    private bool isClickable = false;

    public void Initialize(string panoramaName, string trenObjectName = null, string pultTypeName = null)
    {
        panoramName = panoramaName;
        this.trenObjectName = trenObjectName;
        this.templateTypeName = pultTypeName;
    }

    public void EnableMoving()
    {
        allowMoving = true;
        cameraController.DisableControl();
        EnableHighlight();
    }

    protected override void Cancel()
    {
        if (allowMoving)
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
        {
            allowMoving = false;
        }

        if (!cameraController.IsActive)
        {
            cameraController.EnableControl();
        }

        if (!Approved())
        {
            UIController.Instance.ShowObjectMenu(input.CursorPosition(), this);
            DisableHighlight();
        }
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

        if (allowMoving)
            allowMoving = false;

        if (!cameraController.IsActive)
            cameraController.EnableControl();

        if (UIController.Instance)
        {
            UIController.Instance.ShowTemplateMenu(trenObjectName, templateTypeName);
        }
    }

    private void Update()
    {
        if (allowMoving)
        {
            MoveByCursor();
            CheckArrowBorders();

            if (input.GetFirstKey())
            {
                UIController.Instance.ShowObjectMenu(input.CursorPosition(), this);
                DisableHighlight();
                allowMoving = false;
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

    public string GetPanoramName()
    {
        return panoramName;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetTemplateName()
    {
        return templateTypeName;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public string GetTrenName()
    {
        return trenObjectName;
    }

    public void SetPanoramaName(string name)
    {
        panoramName = name;
    }

    public void SetPultObjectName(string name)
    {
        templateTypeName = name;
    }

    public void SetTrenObjectName(string name)
    {
        trenObjectName = name;
    }

    public void Remove()
    {
        if (gameObject)
        {
            Destroy(gameObject);
            PanoramaController.Instance.RemoveTrenObject(this);
        }
    }

    public TrenObjectInfo GetTrenObjectInfo()
    {
        return new TrenObjectInfo()
        {
            TrenObjectName = trenObjectName,
            TemplateName = templateTypeName,
            PanoramName = panoramName,
            Position = GetPosition(),
            Rotation = GetRotation(),
        };
    }

    public bool Approved()
    {
        return !string.IsNullOrEmpty(trenObjectName) && !string.IsNullOrEmpty(templateTypeName);
    }

    public void SetClickable(bool value)
    {
        isClickable = value;
    }
}
