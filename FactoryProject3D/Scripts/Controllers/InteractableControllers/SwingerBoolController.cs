using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingerBoolController : HandleBoolController, ITrenObjectManipulator
{
    protected override string currentPositivePostfix { get { return "Выключить"; } }
    protected override string currentNegativePostfix { get { return "Включить"; } }

    public float SpeedReturning = 5.0f;
    public float PressAngle = 35.0f;

    private bool currentValue = false;

    private Quaternion originRotation;
    private Quaternion positiveRotation;
    private Quaternion negativeRotation;

    private Coroutine currentCoroutine;

    void Update()
    {
        if (stateCheckingAllowed && currentTrenObjectSet != null)
        {
            var setSim = currentTrenObjectSet.GetSimulatorValue();
            if (currentValue != setSim)
            {
                currentValue = currentTrenObjectSet.GetSimulatorValue();
                if (currentValue)
                {
                    transform.localRotation = positiveRotation;
                }
                else
                {
                    transform.localRotation = negativeRotation;
                }

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = null;
                }

                currentCoroutine = StartCoroutine(Returning());
            }
        }
    }

    public override void Initialize()
    {
        if (InitializeTrenObject(out currentTrenObjectSet, out currentTrenObjectGet))
        {
            InitializeRotations();
            Enable();
        }
        else
        {
            Disable();
        }
    }
    private void InitializeRotations()
    {
        originRotation = transform.localRotation;
        positiveRotation = Quaternion.Euler(transform.localEulerAngles + Vector3.right * PressAngle);
        negativeRotation = Quaternion.Euler(transform.localEulerAngles - Vector3.right * PressAngle);
    }

    public void UpdateTrenObject()
    {
        if (currentTrenObjectSet != null)
        {
            var trenObject = currentTrenObjectSet as TrenObject;
            if (trenObject)
            {
                trenObject.UpdateSetter();
                trenObject.UpdateGetter();
                currentValue = currentTrenObjectSet.GetSimulatorValue();
            }
        }
    }

    private IEnumerator Returning()
    {
        var time = 0.0f;
        var startRotation = transform.localRotation;
        var endRotation = originRotation;

        while (time < 1.0f)
        {
            time += Time.deltaTime * SpeedReturning;

            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, time);

            yield return new WaitForEndOfFrame();
        }

        currentCoroutine = null;
    }    
}
