using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : HandleBoolController, IAdvancedInteractableController
{
    protected override string currentPositivePostfix { get { return customPositivePostfix; } }
    protected override string currentNegativePostfix { get { return customNegativePostfix; } }

    [Header("Messages:")]

    [SerializeField]
    private string customPositivePostfix = "Выключить";

    [SerializeField]
    private string customNegativePostfix = "Включить";

    [SerializeField]
    private string customName;

    [Space(10)]
    [Header("Pressing Settings:")]

    [SerializeField]
    private float offset = 0.005f;

    [SerializeField]
    private Vector3 offsetAxis = Vector3.back;

    [SerializeField]
    private float pressingSpeed = 5.0f;

    [Space(10)]
    [Header("Value Sending Settings:")]

    [SerializeField]
    private bool isSticky = false;

    [SerializeField]
    private bool sendOneValue = false;

    [SerializeField]
    private bool sendingValue = false;

    [SerializeField]
    private bool isHolded = false;

    private Vector3 originPosition;
    private Vector3 pressedPosition;

    private Coroutine currentRoutine;

    private bool currentValue = false;

    private void OnValidate()
    {
        if (customName != null)
        {
            customName = customName.Trim(); 
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        originPosition = transform.localPosition;
        pressedPosition = transform.localPosition - transform.InverseTransformDirection(offsetAxis * offset);

        if (currentTrenObjectGet != null)
        {
            currentValue = currentTrenObjectGet.GetSimulatorValue();
            if (isSticky)
            {
                transform.localPosition = currentValue ? pressedPosition : originPosition;
            }
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        if (!sendOneValue || isHolded)
        {
            base.Interact((bool?)value, withSimulator);
        }
        else
        {
            base.Interact(sendingValue, withSimulator);
        }

        if (!isSticky && !isHolded)
        {
            Press();
        }
    }

    public override string GetTrenName()
    {
        if (string.IsNullOrEmpty(customName))
        {
            return base.GetTrenName();
        }
        else
        {
            return customName;
        }
    }

    private void Update()
    {
        if (isSticky)
        {
            if (currentTrenObjectGet != null && currentValue != currentTrenObjectGet.GetSimulatorValue())
            {
                currentValue = currentTrenObjectGet.GetSimulatorValue();
                Press(currentValue);
            }
        }
    }

    private void Press()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(StartPressing());
    }
    private void Press(bool value)
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(value ? StartPressing() : StartUnpressing());
    }
    private IEnumerator StartPressing()
    {
        float time = 0.0f;
        var startPosition = transform.localPosition;
        while (time < 1.0f)
        {
            time += Time.deltaTime * pressingSpeed;
            transform.localPosition = Vector3.Lerp(startPosition, pressedPosition, time);
            yield return new WaitForEndOfFrame();
        }

        currentRoutine = isSticky || isHolded ? null : StartCoroutine(StartUnpressing());
    }
    private IEnumerator StartUnpressing()
    {
        float time = 0.0f;
        var startPosition = transform.localPosition;
        while (time < 1.0f)
        {
            time += Time.deltaTime * pressingSpeed * 0.5f;
            transform.localPosition = Vector3.Lerp(startPosition, originPosition, time);
            yield return new WaitForEndOfFrame();
        }

        currentRoutine = null;
    }
    private void StopCurrentRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    public override string GetPostfix()
    {
        if (!sendOneValue)
        {
            return base.GetPostfix();
        }
        else
        {
            if (sendingValue)
            {
                return currentNegativePostfix;
            }
            else
            {
                return currentPositivePostfix;
            }
        }
    }

    public void StartInteraction()
    {
        if (isHolded)
        {
            Interact(sendingValue);
            Press(true);
        }
    }

    public void StopInteraction()
    {
        if (isHolded)
        {
            Interact(!sendingValue);
            Press(false);
        }
        else
        {
            Interact(sendingValue);
        }
    }
}
