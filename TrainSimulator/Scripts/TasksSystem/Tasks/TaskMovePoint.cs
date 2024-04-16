using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMovePoint : BaseTask
{
    [SerializeField] private MovePoint _movePoint;
    private TargetVisualInfo _targetVisualInfo;
    protected override void LaunchTask()
    {
        base.LaunchTask();
        _movePoint.gameObject.SetActive(true);
        _movePoint.OnPlayerReach += PlayerReachPoint;
    }

    private void PlayerReachPoint()
    {
        FireCompleteSignal();
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        base.OnStartInteract(signal);
        FireHardWrong();
    }

    public override void HiglightEnable()
    {
        base.HiglightEnable();
        _movePoint.EnableVisual(true);
        
        _targetVisualInfo = new TargetVisualInfo()
        {
            OffscreenOffset = _movePoint.OffscreenOffset,
            Sprite = _movePoint.Sprite
        };
        
        _higlightHolder.AddOffscreenTargetWithoutEntity(_movePoint.gameObject,_targetVisualInfo);
    }

    public override void HiglightDisable()
    {
        base.HiglightDisable();
        _movePoint.EnableVisual(false);
    }

    public override void Dispose()
    {
        base.Dispose();
        _movePoint.OnPlayerReach -= PlayerReachPoint;
        _movePoint.gameObject.SetActive(false);
    }
}
