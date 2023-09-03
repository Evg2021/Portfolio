using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeJointController : TrenObjectControllerBase
{
    public Vector3 AxisY = Vector3.up;
    public float AngleRotation = 90.0f;
    public float RotationSpeed = 1.0f;

    private Quaternion startRotation;
    private Quaternion endRotation;
    private Coroutine currentHingeJointRotationRoutine;
    private bool isClosed = false;

    unsafe private bool* getBool;
    unsafe private bool* setBool;

    void Start()
    {
        //startRotation = Quaternion.identity;
        startRotation = transform.localRotation;
        endRotation = startRotation * Quaternion.Euler(AxisY * AngleRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator StartHingeJointRotation(Quaternion startPosition, Quaternion endPosition)
    {
        float time = 0.0f; 

        while (time < 1.0f)
        {
            time += Time.deltaTime * RotationSpeed;
            transform.localRotation = Quaternion.Lerp(startPosition, endPosition, time);
            yield return new WaitForEndOfFrame();
        }
        currentHingeJointRotationRoutine = null;
    }
    
}
