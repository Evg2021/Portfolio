using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : EnvironmentControllerBase, ICustomInfoController
{
    private const string positivePostfix = "закрыть дверь";
    private const string negativePostfix = "открыть дверь";

    public Vector3 AxisRotation = Vector3.forward;
    public float AngleRotation = 120.0f;

    public float Speed = 2.0f;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    public NetworkVariable<bool> IsOpened = new NetworkVariable<bool>();

    private Coroutine currentRoutine;
    private StarterAssetsInputs input;

    private void Awake()
    {
        closedRotation = transform.rotation;
        openedRotation = Quaternion.Euler(closedRotation.eulerAngles + AxisRotation * AngleRotation);        
    }

    public void CloseDoor()
    {
        StopRotating();
        currentRoutine = StartCoroutine(RotateDoor(openedRotation, closedRotation));
        CurrentValue = false;
        IsOpened.Value = false;
    }

    public void OpenDoor()
    {
        StopRotating();
        currentRoutine = StartCoroutine(RotateDoor(closedRotation, openedRotation));
        CurrentValue = true;
        IsOpened.Value = true;
    }

    private IEnumerator RotateDoor(Quaternion fromRotation, Quaternion toRotation)
    {
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime * Speed;
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);
            yield return new WaitForEndOfFrame();
        }
        currentRoutine = null;
    }

    private void StopRotating()
    {
        if(currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    public override void Interact()
    {
        if (CurrentValue)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    public override void SetState(bool state)
    {
        if (CurrentValue != state)
        {
            Interact();
        }
    }

    public string GetPostfix()
    {
        if (IsOpened.Value)
        {
            return positivePostfix;
        }
        else
        {
            return negativePostfix;
        }
    }
}
