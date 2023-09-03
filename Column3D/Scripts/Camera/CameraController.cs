using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static bool isMoving { get; private set; }

    public AnimationCurve CurveMovement;
    public float Speed = 1.0f;

    [Header("Control Settings:")]
    public Vector2 ControlSpeed = Vector2.one;
    public float ScrollingSpeed = 0.2f;
    public float MaxHeight = 38.0f;
    public float MinHeight = 5.0f;
    public float MaxDistance = 75.0f;
    public float MinDistance = 2.0f;

    private Coroutine currentMoving;
    private Coroutine currentRotation;
    private Action currentAction;
    private bool currentActionIsInvoked;
    private float currentDistance = 1.0f;
    private bool isAllowToMove;
    private Vector2 startMousePosition;
    private bool isControllable;
    private bool isRotateAroundAllow;

    private static float zRotationLimit = 0.0f;
    private static float distanceAccuracy = 0.001f;

    private void Awake()
    {
        isControllable = false;
        isAllowToMove = false;
        isRotateAroundAllow = false;
    }

    public void ClosePreviuseTrack()
    {
        if (currentMoving != null)
        {
            StopCoroutine(currentMoving);
            currentMoving = null;
            isMoving = false;
        }

        if (currentRotation != null)
        {
            StopCoroutine(currentRotation);
            currentRotation = null;
        }

        if (!currentActionIsInvoked)
        {
            currentAction?.Invoke();
            currentActionIsInvoked = true;
        }
    }

    public void PlayTrack(Track track, Action action = null, Transform transitionPoint = null)
    {
        ClosePreviuseTrack();

        isMoving = true;

        if (transitionPoint != null)
        {
            currentMoving = StartCoroutine(MoveToPoint(track.ViewPosition, transitionPoint.position));
            currentRotation = StartCoroutine(RotateToTarget(track.ViewRotation, transitionPoint.rotation));
        }
        else
        {
            currentMoving = StartCoroutine(MoveToPoint(track.ViewPosition));
            currentRotation = StartCoroutine(RotateToTarget(track.ViewRotation));
        }

        currentActionIsInvoked = false;
        currentAction = action;
    }

    public void EnableControll()
    {
        ResetParentAngles();
        isControllable = true;
    }
    public void DisableControll()
    {
        isControllable = false;
        isMoving = false;
    }

    private IEnumerator MoveToPoint(Vector3 position)
    {
        var time = 0.0f;
        var oldPosition = transform.position;
        currentDistance = Mathf.Sqrt((position - oldPosition).magnitude);

        if (currentDistance <= distanceAccuracy)
        {
            currentDistance = distanceAccuracy;
        }

        while (time <= 1.0f)
        {
            time += (Time.deltaTime * Speed) / currentDistance;
            transform.position = Vector3.Lerp(oldPosition, position, CurveMovement.Evaluate(time));
            yield return new WaitForEndOfFrame();
        }

        currentAction?.Invoke();
        currentActionIsInvoked = true;
        currentMoving = null;
        isMoving = false;
    }
    private IEnumerator MoveToPoint(Vector3 position, Vector3 transitionPosition)
    {
        var time = 0.0f;
        var oldPosition = transform.position;
        currentDistance = Mathf.Sqrt(((transitionPosition - oldPosition).magnitude + (position - transitionPosition).magnitude)* 0.5f);

        if (currentDistance <= distanceAccuracy)
        {
            currentDistance = 1.0f;
        }

        while (time <= 1.0f)
        {
            time += (Time.deltaTime * Speed) / currentDistance;
            transform.position = Vector3.Lerp(Vector3.Lerp(oldPosition, transitionPosition, CurveMovement.Evaluate(time)), 
                                              Vector3.Lerp(transitionPosition, position, CurveMovement.Evaluate(time)), 
                                              CurveMovement.Evaluate(time));
            yield return new WaitForEndOfFrame();
        }

        currentAction?.Invoke();
        currentActionIsInvoked = true;
        currentMoving = null;
        isMoving = false;
    }

    private IEnumerator RotateToTarget(Quaternion rotation)
    {
        var time = 0.0f;
        var oldRotation = transform.rotation;
        while(time <= 1.0f)
        {
            time += (Time.deltaTime * Speed) / currentDistance;
            var newRotation = Quaternion.Lerp(oldRotation, rotation, CurveMovement.Evaluate(time));
            transform.rotation = Quaternion.Euler(newRotation.eulerAngles.x, newRotation.eulerAngles.y, Mathf.Clamp(newRotation.eulerAngles.z, -zRotationLimit, zRotationLimit));

            yield return new WaitForEndOfFrame();
        }

        currentRotation = null;
    }
    private IEnumerator RotateToTarget(Quaternion rotation, Quaternion transitionRotation)
    {
        var time = 0.0f;
        var oldRotation = transform.rotation;
        while (time <= 1.0f)
        {
            time += (Time.deltaTime * Speed) / currentDistance;
            var newRotation = Quaternion.Lerp(Quaternion.Lerp(oldRotation, transitionRotation, CurveMovement.Evaluate(time)),
                                                 Quaternion.Lerp(transitionRotation, rotation, CurveMovement.Evaluate(time)),
                                                 CurveMovement.Evaluate(time));
            transform.rotation = Quaternion.Euler(newRotation.eulerAngles.x, newRotation.eulerAngles.y, Mathf.Clamp(newRotation.eulerAngles.z, -zRotationLimit, zRotationLimit));

            yield return new WaitForEndOfFrame();
        }

        currentRotation = null;
    }

    private void ResetParentAngles()
    {
        if (transform.parent != null)
        {
            var parent = transform.parent;
            transform.SetParent(null);
            parent.eulerAngles = Vector3.up * transform.eulerAngles.y;
            transform.parent = parent;
        }
    }

    private void Update()
    {
        if (isControllable && transform.parent != null)
        {
            var distance = Mathf.Abs(transform.localPosition.z);

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !isRotateAroundAllow)
            {
                startMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.dpi;
                isAllowToMove = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isAllowToMove = false;
            }

            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && !isAllowToMove)
            {
                startMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.dpi;
                isRotateAroundAllow = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isRotateAroundAllow = false;
            }

            if (isAllowToMove || isRotateAroundAllow)
            {
                var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) / Screen.dpi;
                var diffX = currentMousePosition.x - startMousePosition.x;
                var diffY = currentMousePosition.y - startMousePosition.y;

                if (isAllowToMove)
                {
                    transform.parent.Rotate(Vector3.up, diffX * ControlSpeed.x * Mathf.Sqrt(distance));
                    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y - (diffY * ControlSpeed.y * distance), MinHeight, MaxHeight), transform.position.z);
                }

                if (isRotateAroundAllow)
                {
                    transform.rotation = Quaternion.Euler(transform.eulerAngles.x - diffY * ControlSpeed.x,
                                                          transform.eulerAngles.y + diffX * ControlSpeed.x, 
                                                          transform.eulerAngles.z);
                }

                startMousePosition = currentMousePosition;
            }

            var scrolling = -Input.mouseScrollDelta.y * ScrollingSpeed * distance;
            var newDistance = Mathf.Clamp(Mathf.Abs(transform.localPosition.z) + scrolling, MinDistance, MaxDistance);
            if (transform.localPosition.z < 0)
            {
                newDistance *= -1;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newDistance);

            isMoving = isAllowToMove || scrolling > 0 || scrolling < 0;
        }
    }
}
