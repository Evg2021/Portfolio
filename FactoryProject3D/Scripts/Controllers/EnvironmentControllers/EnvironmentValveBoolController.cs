using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentValveBoolController : EnvironmentHandleBoolController, ICustomInfoController
{
    [Space(10)]

    [SerializeField]
    private float RotationsPerPress = 2;

    [SerializeField]
    private float RotationSpeed = 3;

    [Space(10)]

    [SerializeField]
    private string positivePostfix = "перекрыть ";

    [SerializeField]
    private string negativePostfix = "открыть ";

    [SerializeField]
    private string generalPostfix = "поток на ";

    private Coroutine currentCoroutine;
    private float currentValveValue = 0.0f;
    private float rotationSpeedKoeff = 100.0f;

    public override void Initialize(uint index)
    {
        base.Initialize(index);
        currentValveValue = RotationsPerPress * 360.0f;
    }

    public override void Interact()
    {
        base.Interact();

        StopCurrentRoutine();
        currentCoroutine = StartCoroutine(Rotate(!CurrentValue));
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

    private IEnumerator Rotate(bool direction = true)
    {
        float targetValue = direction ? RotationsPerPress * 360 : 0.0f;
        while (currentValveValue != targetValue)
        {
            var newValue = RotationSpeed * Time.deltaTime * (direction ? 1 : -1) * rotationSpeedKoeff;
            currentValveValue = Mathf.Clamp(currentValveValue + newValue, 0.0f, RotationsPerPress  * 360);
            transform.Rotate(Vector3.up, newValue);
            yield return new WaitForEndOfFrame();
        }
        currentCoroutine = null;
    }
    private void StopCurrentRoutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }
}
