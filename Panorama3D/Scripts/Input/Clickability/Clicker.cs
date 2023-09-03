using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clicker : SingletonMonoBehaviour<Clicker>
{
    public static bool EditorMode { get; private set; }

    [SerializeField]
    private Texture2D CursorImage;

    [SerializeField]
    private bool editorMode = true;

    private new Camera camera;
    private InputBase input;

    private IClickable currentObject;

    private bool isHold;
    private Vector2 lastPositionOnDown;

    private bool isActive;

    protected override void Awake()
    {
        base.Awake();
        EditorMode = editorMode;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        input = InputBase.Instance;
        isHold = false;

        input.FirstKeyIsDown  += Input_FirstKeyIsDown;
        input.FirstKeyIsUp    += Input_FirstKeyIsUp;
        input.SecondKeyIsDown += Input_SecondKeyIsDown;
        input.FirstKeyIsHold  += Input_FirstKeyIsHold;

        isActive = true;
    }

    public void Cancel()
    {
        currentObject = null;
    }

    public void Enable()
    {
        isActive = true;
    }

    public void Disable()
    {
        isActive = false;
    }

    private void Input_FirstKeyIsHold(Vector2 obj)
    {
        if (!isActive || !editorMode) return;

        if (currentObject != null)
        {
            currentObject.OnHold();

            if (!isHold)
                isHold = true;
        }
    }

    private void Input_SecondKeyIsDown(Vector2 obj)
    {
        if (!isActive || !editorMode) return;

        if (Input.GetMouseButton(0)) return;

        var clickableObject = GetClickableObject(obj);
        if(clickableObject != null)
        {
            clickableObject.OnClickRight();
        }
    }

    private void Update()
    {
        if(GetClickableObject(input.CursorPosition()) != null)
        {
            SetCursor(CursorImage);
        }
        else
        {
            SetCursor(null);
        }
    }

    private void SetCursor(Texture2D cursorImage)
    {
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void OnDestroy()
    {
        input.FirstKeyIsDown  -= Input_FirstKeyIsDown;
        input.FirstKeyIsUp    -= Input_FirstKeyIsUp;
        input.SecondKeyIsDown -= Input_SecondKeyIsDown;
        input.FirstKeyIsHold  -= Input_FirstKeyIsHold;
    }

    private IClickable GetClickableObject(Vector2 position)
    {
        if (!isActive) return null;

        if (EventSystem.current.IsPointerOverGameObject())
            return null;

        return GetClickableObject(position, camera);
    }

    public static IClickable GetClickableObject(Vector2 position, Camera camera, bool isUVPositions = false)
    {
        var ray = new Ray();
        if (!isUVPositions)
        {
            ray = camera.ScreenPointToRay(position);
        }
        else
        {
            ray = camera.ViewportPointToRay(position);
        }

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.TryGetComponent(typeof(IClickable), out var component))
            {
                return ((IClickable)component);
            }

            var clickable = hit.transform.GetComponentInParent(typeof(IClickable));

            if (clickable != null)
            {
                return ((IClickable)clickable);
            }

            clickable = hit.transform.GetComponentInChildren(typeof(IClickable));

            if (clickable != null)
            {
                return ((IClickable)clickable);
            }
        }

        return null;
    }

    private void Input_FirstKeyIsDown(Vector2 obj)
    {
        if (!isActive) return;

        currentObject = GetClickableObject(obj);
        if (currentObject != null)
        {
            currentObject.OnPressDown();
            lastPositionOnDown = obj;
        }
    }

    private void Input_FirstKeyIsUp(Vector2 obj)
    {
        if (!isActive) return;

        if (currentObject == null) return;

        if (obj == lastPositionOnDown)
        {
            var clickable = GetClickableObject(obj);
            if (clickable == currentObject)
                currentObject.OnPressUp();
        }
        else
        {
            currentObject.CancelPress();
        }

        currentObject = null;

        isHold = false;
    }
}
