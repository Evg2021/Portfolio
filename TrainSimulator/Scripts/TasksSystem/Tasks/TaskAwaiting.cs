using System.Collections;
using UnityEngine;

public class TaskAwaiting : BaseTask
{
    public float TimerAwait => _timerAwait;

    [SerializeField] protected float _timerAwait;
    protected IEnumerator _coroutine;

    protected override void LaunchTask()
    {
        base.LaunchTask();
        _coroutine = Await();
        StartCoroutine(_coroutine);
    }

    protected virtual IEnumerator Await()
    {
        yield return new WaitForSeconds(_timerAwait);

        TimerOutAction();
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        FireHardWrong();
    }

    protected override void OnStopInterract(InteractableSignal signal)
    {
       // FireWrong();
    }

    protected virtual void TimerOutAction()
    {
        FireCompleteSignal();
    }
    
    public override void Dispose()
    {
        base.Dispose();
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }
}