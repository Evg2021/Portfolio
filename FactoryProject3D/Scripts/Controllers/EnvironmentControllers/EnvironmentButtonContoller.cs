using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnvironmentButtonContoller : EnvironmentHandleBoolController, ICustomInfoController
{
    [Header("Settings:")]
    public Vector3 PressAxis = Vector3.right;
    public float PressOffset = 0.01f;
    public float PressSpeed = 2.0f;

    [Space(10)]

    [SerializeField]
    private string positivePostfix = "выключить";

    [SerializeField]
    private string negativePostfix = "включить";

    [SerializeField]
    private string generalPostfix = string.Empty;

    private Vector3 originPosition;
    private Vector3 pressedPosition;

    private Coroutine currentCoroutine;

    public override void Initialize(uint index)
    {
        base.Initialize(index);
        InitializePositions();

    }
    private void InitializePositions()
    {
        originPosition = transform.localPosition;
        pressedPosition = transform.localPosition + PressAxis * PressOffset;
    }

    public override void Interact()
    {
        base.Interact();

        StopCurrentCoroutine();
        currentCoroutine = StartCoroutine(Press());
    }

    private IEnumerator Press()
    {
        float lerpValue = 0.0f;
        Vector3 startPosition = transform.localPosition;

        while (lerpValue < 1.0f)
        {
            lerpValue += Time.deltaTime * PressSpeed;
            transform.localPosition = Vector3.Lerp(startPosition, pressedPosition, lerpValue);
            yield return new WaitForEndOfFrame();
        }

        currentCoroutine = StartCoroutine(Returning());
    }
    private IEnumerator Returning()
    {
        float lerpValue = 0.0f;
    
        while (lerpValue < 1.0f)
        {
            lerpValue += Time.deltaTime * PressSpeed;
            transform.localPosition = Vector3.Lerp(pressedPosition, originPosition, lerpValue);
            yield return new WaitForEndOfFrame();
        }

        currentCoroutine = null;
    }
    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    public string GetPostfix()
    {
        if (CurrentValue)
        {
            return positivePostfix + generalPostfix;
        }
        else
        {
            return negativePostfix + generalPostfix;
        }
    }
}
