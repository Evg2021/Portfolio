using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRotatableBoolController : HandleBoolController
{
    public Vector3 Axis = Vector3.forward;    
    public float AngleRotation = 180.0f;      
    public float RotationSpeed = 1.0f;

    private Quaternion startRotation;
    private Quaternion endRotation;
    private Coroutine currentDamperRotationRoutine;

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);

        startRotation = transform.localRotation;
        endRotation = startRotation * Quaternion.Euler(Axis * AngleRotation);

        transform.localRotation = currentValue ? endRotation : startRotation;
    }

    protected override void OnChangedValue()
    {
        Rotate();
    }

    public void Rotate() 
    {        
        StopCurrentDamperRotationRoutine();
        currentDamperRotationRoutine = StartCoroutine(StartDamperRotation(transform.localRotation, currentValue ? endRotation : startRotation));
    }
    private IEnumerator StartDamperRotation(Quaternion startPosition, Quaternion endPosition)
    {
        float time = 0.0f;        

        while (time < 1.0f)
        {
            time += Time.deltaTime * RotationSpeed;
            transform.localRotation = Quaternion.Lerp(startPosition, endPosition, time);
            yield return new WaitForEndOfFrame();
        }
        currentDamperRotationRoutine = null;
    }
    private void StopCurrentDamperRotationRoutine()
    {
        if(currentDamperRotationRoutine != null)
        {
            StopCoroutine(currentDamperRotationRoutine);
            currentDamperRotationRoutine = null;
        }
    }
}
