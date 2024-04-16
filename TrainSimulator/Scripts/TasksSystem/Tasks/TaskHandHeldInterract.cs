using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TaskHandHeldInterract : TaskEntity
{
    private HandHeldObjectSpawner _handHeldObjectSpawner;

    [Inject]
    protected virtual void Construct(HandHeldObjectSpawner handHeldObjectSpawner)
    {
        _handHeldObjectSpawner = handHeldObjectSpawner;
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        foreach (var i in _entities)
        {
            bool isContain = _handHeldObjectSpawner.IsContainEntity(signal.Value);
            bool IsMatch = _handHeldObjectSpawner.GetHandHeldByPrefab(i) ==
                           _handHeldObjectSpawner.CurrentHandHeld;

            if (isContain && IsMatch)
                FireCompleteSignal();
            else
                FireHardWrong();
        }
    }

    public override void HiglightEnable()
    {
        foreach (var i in _entities)
        {
            if(_enableOutline)
                _higlightHolder.EnableOultines(_handHeldObjectSpawner.GetHandHeldByPrefab(i).Outline);
            if(_enableOffscreenArrow)
                _higlightHolder.AddOffscreenTarget(_handHeldObjectSpawner.GetHandHeldByPrefab(i));
        }
    }
}