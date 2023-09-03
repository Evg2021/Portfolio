using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveController : ControllerBase
{
    private const string audioClipPath = "Sounds/squeaky_open";
    private const string floatHManipulatorName = "FloatHManipulator";
    private const float floatHRotationsToFull = 0.25f;

    public float angleTest;

    [SerializeField]
    private float RotationsToFull = 3.5f;

    [SerializeField]
    private float RotationSpeed = 10.0f;

    private float rotationsPercent
    {
        get
        {
            if (RotationsToFull != 0)
            {
                return (RotationsToFull * 360) / maxValue;
            }
            return 1;
        }
    }

    private Transform manipulator;
    private AudioSource audioSource;

    private ITrenInteractableFloat currentTrenObjectSet;
    private ITrenInteractableFloat currentTrenObjectGet;
    private float currentValue = 0.0f;

    private static float maxValue = 100.0f;
    private static float minValue = 0.0f;
    private static AudioClip sound;

    private Coroutine currentRotationRoutine;
    private Vector3 rotationAxis;

    public bool isGearing { get; private set; }

    private void Reset()
    {
        if (name.Contains(floatHManipulatorName))
        {
            RotationsToFull = floatHRotationsToFull;
        }
    }

    public override dynamic GetSimulatorValue()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetSimulatorValue();
        }

        return null;
    }

    private void Update()
    {
        if (currentTrenObjectGet != null)
        {
            if (currentTrenObjectGet.GetSimulatorValue() != currentValue)
            {
                float difference = currentTrenObjectGet.GetSimulatorValue() - currentValue;
                currentValue = currentTrenObjectGet.GetSimulatorValue();
                RotateMesh(difference);
            }
        }
    }

    public override void Interact(dynamic value, bool withSimulator = true)
    {
        if (isEnabled)
        {
            if (withSimulator && currentTrenObjectSet != null && currentTrenObjectGet != null)
            {
                if (value < 0.0f && currentTrenObjectGet.GetSimulatorValue() + value >= minValue ||
                    value > 0.0f && currentTrenObjectGet.GetSimulatorValue() + value <= maxValue  )
                {
                    currentTrenObjectSet.Interact(value);

                    //RotateMesh(value);
                }
            }
            /*else if (!withSimulator)
            {
                RotateMesh(value);

                if (currentTrenObjectSet != null)
                {
                    currentTrenObjectSet.ChangeSetter(value);
                }
            }*/
        }
    }

    private void RotateMesh(float value)
    {
        if (manipulator)
        {
            float rotate = value * rotationsPercent;
            StopCurrentRotationRoutine();
            currentRotationRoutine = StartCoroutine(StartRotation(rotate));
        }

        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    private IEnumerator StartRotation(float value)
    {
        float time = 0.0f;
        float newRotation = 0.0f;
        float oldRotation = 0.0f;

        while (time < 1.0f)
        {
            time += Time.deltaTime * RotationSpeed;
            oldRotation = newRotation;
            newRotation = Mathf.Lerp(0.0f, value, time);
            var angle = newRotation - oldRotation;
            angleTest = angle;
            manipulator.Rotate(Vector3.down, angle);
            yield return new WaitForEndOfFrame();
        }

        currentRotationRoutine = null;
    }
    private void StopCurrentRotationRoutine()
    {
        if (currentRotationRoutine != null)
        {
            StopCoroutine(currentRotationRoutine);
            currentRotationRoutine = null;
        }
    }

    public override void Initialize()
    {
        Gear();
        InitializeManipualtor();
        if (InitializeTrenObject(out currentTrenObjectSet, out currentTrenObjectGet))
        {
            float difference = currentTrenObjectGet.GetSimulatorValue() - currentValue;
            currentValue = currentTrenObjectGet.GetSimulatorValue();
            
            if (manipulator)
            {
                rotationAxis = -manipulator.up;
            }

            RotateMesh(difference);
            InitializeSound();
            Enable();
        }
        else
        {
            Disable();
        }
    }

    private void InitializeManipualtor()
    {
        manipulator = transform;
    }
    private void InitializeSound()
    {
        if (sound == null)
        {
            sound = Resources.Load<AudioClip>(audioClipPath);
        }

        if (sound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound;
        }
    }

    public override Types GetControllerType()
    {
        return Types.TYPE_FLOAT;
    }

    public override string GetTrenName()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetTrenName();
        }

        return null;
    }

    public void Gear()
    {
        if (!isGearing)
        {
            isGearing = true;
        }
    }

    public void Ungear()
    {
        if (isGearing)
        {
            isGearing = false;
        }
    }

    public override uint GetTrenObjectIndex()
    {
        if (currentTrenObjectGet != null)
        {
            return currentTrenObjectGet.GetTrenIndex();
        }

        return uint.MaxValue;
    }
}
