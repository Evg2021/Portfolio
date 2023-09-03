using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnPressDown;
    public event Action OnPressUp;
    public event Action OnCursorEnter;
    public event Action OnCursorExit;

    public Vector3 EulerAngles
    {
        get
        {
            return rotationObject.localEulerAngles;
        }
        set
        {
            rotationObject.localEulerAngles = value;
        }
    }

    private bool cursorInsideImage = false;
    private bool cursorIsHolded = false;

    private float speed        =  20.0f;
    private float minVertAngle = -90.0f;
    private float maxVertAngle =  90.0f;

    private Vector3 startRotation;
    private Transform rotationObject;

    public void Initialize(Transform rotationObject)
    {
        this.rotationObject = rotationObject;
    }

    public void ResetRotation()
    {
        rotationObject.localEulerAngles = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorInsideImage = true;
        OnCursorEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorInsideImage = false;
        OnCursorExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cursorIsHolded = true;
        startRotation = rotationObject.localEulerAngles;

        OnPressDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cursorIsHolded = false;

        OnPressUp?.Invoke();
    }

    public void Rotate(Vector2 diffPosition)
    {
        if (cursorInsideImage && cursorIsHolded)
        {
            if (rotationObject == null) return;

            var newEulerAngles = startRotation + new Vector3(diffPosition.y, -diffPosition.x, 0) * speed;

            var unclampedWrapedX = Utilities.WrapAngle(newEulerAngles.x);
            var clampedUnwrapX = Utilities.UnwrapAngle(Mathf.Clamp(unclampedWrapedX, minVertAngle, maxVertAngle));
            newEulerAngles.x = clampedUnwrapX;

            rotationObject.localEulerAngles = newEulerAngles;
        }
    }
}
