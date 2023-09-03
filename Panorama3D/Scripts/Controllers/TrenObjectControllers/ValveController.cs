using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveController : TrenObjectControllerBase
{
    public bool IsDependOnGetSimulatorValue = false; 

    private const float maxValue = 100.0f;
    private const float minValue = 0.0f;

    [SerializeField]
    private float rotationsToFull = 3.5f;

    [SerializeField]
    private float rotationSpeed = 10.0f;

    [SerializeField]
    private float currentValue;
    
    private Coroutine currentRotationRoutine;
    private Coroutine currentStartSyncMeshCoroutine;
     
    private ValueFloatShowerController currentShower;


    unsafe private float* getFloat;
    unsafe private float* setFloat;

    private float rotationsPer—ent 
    {
        get
        {
            if (rotationsToFull != 0)
            {
                return (rotationsToFull * 360) / maxValue;
            }
            return 1;
        }
    }

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);

        getFloat = (float*)get;
        setFloat = (float*)set;

        ClientSocketManager.UpdateGetOne(Index);

        *setFloat = *getFloat;
                
        currentShower = TryFindValueShower();

        if (IsDependOnGetSimulatorValue)
        {
            currentStartSyncMeshCoroutine = StartCoroutine(StartSyncMeshRoutine());
        }        
    }

    private void Update()
    {
        if (isActive)
        {
            var scrollDelta = InputBase.Instance.ScrollDelta();
            unsafe
            {
                if (!IsDependOnGetSimulatorValue)
                {
                    var checkValue = *setFloat + scrollDelta;
                    checkValue = Mathf.Clamp(checkValue, minValue, maxValue);
                    *setFloat = checkValue;                    
                }
                else
                {
                    *setFloat += scrollDelta;
                }

                ClientSocketManager.UpdateSetOne(Index);
            }
        }
        unsafe
        {
            ClientSocketManager.UpdateGetOne(Index);

            if (IsDependOnGetSimulatorValue)
            {
                if (currentShower != null && currentValue != currentShower.SimulatorValue)
                {
                    Rotate(currentValue - currentShower.SimulatorValue);
                    currentValue = currentShower.SimulatorValue;
                }
            }
            else
            {
                if (currentValue != *getFloat)
                {
                    Rotate(currentValue - *getFloat);
                    currentValue = *getFloat;
                }
            }
        }
    }

    public void Rotate(float value)
    {
        float rotationValue = value * rotationsPer—ent;  
        currentRotationRoutine = StartCoroutine(StartRotation(rotationValue));
    }

    public void StartSyncMesh()
    {
        transform.Rotate(Vector3.up, currentValue * rotationsPer—ent);        
    }

    private IEnumerator StartSyncMeshRoutine()
    {
        while (currentShower && !currentShower.isInitilaize)
        {
            yield return new WaitForEndOfFrame();
        }

        if (currentShower && currentShower.isInitilaize)
        {
            currentValue = currentShower.SimulatorValue;
            StartSyncMesh();
        }
        currentStartSyncMeshCoroutine = null;
    }

    private IEnumerator StartRotation(float value)
    { 
        float time = 0.0f;
        float newRotation = 0.0f;
        float oldRotation;

        while (time < 1.0f)
        {
            time += Time.deltaTime * rotationSpeed;
            oldRotation = newRotation;
            newRotation = Mathf.Lerp(0.0f, value, time);
            var angle = newRotation - oldRotation;
            transform.Rotate(Vector3.up, angle);
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

    private ValueFloatShowerController TryFindValueShower()
    {
        return TryFindComponentInNeighbors<ValueFloatShowerController>();
    }

    private T TryFindComponentInNeighbors<T>()
        where T : Component
    {
        return transform.parent.parent?.GetComponentInChildren<T>();
    }
}
