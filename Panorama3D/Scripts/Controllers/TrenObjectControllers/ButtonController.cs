using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : HandleBoolController
{    
    public float OffsetPosition;
    public float SpeedOfPushing;
    public float SpeedOfBackPushing;
    public Vector3 PushingAxis = Vector3.forward;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Coroutine currentPushButtonRoutine;

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);

        startPosition = transform.localPosition;
        endPosition = startPosition + PushingAxis * OffsetPosition;
    }

    protected override void OnChangedValue()
    {
        PushButton();
    }

    public void PushButton()
    {
        StopCurrentOnClickButtonRoutine();
        currentPushButtonRoutine = StartCoroutine(OnClickButton(startPosition, endPosition));
    }
    private IEnumerator OnClickButton(Vector3 start, Vector3 end)
    {
        float time = 0.0f;
        Vector3 currentStartPosition = transform.localPosition;
        while(time < 1)
        {
            time += Time.deltaTime * SpeedOfPushing;
            transform.localPosition = Vector3.Lerp(currentStartPosition, end, time);
            yield return new WaitForEndOfFrame();   
        }
        time = 0.0f;
        while (time < 1)
        {
            time += Time.deltaTime * SpeedOfBackPushing;
            transform.localPosition = Vector3.Lerp(end, start, time);
            yield return new WaitForEndOfFrame();
        }

        currentPushButtonRoutine = null;
    }  
    private void StopCurrentOnClickButtonRoutine()
    {
        if(currentPushButtonRoutine != null)
        {
            StopCoroutine(currentPushButtonRoutine);
            currentPushButtonRoutine = null;
        }
    }
    private void OnMouseDown()
    {
        currentPushButtonRoutine = StartCoroutine(OnClickButton(startPosition, endPosition));
    }

}
