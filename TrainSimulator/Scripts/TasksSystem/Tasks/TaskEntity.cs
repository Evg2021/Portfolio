using System.Collections.Generic;
using UnityEngine;

public abstract class TaskEntity : BaseTask
{
    [Header("Highlight settings")] [SerializeField]
    protected bool _enableOutline = true;

    [SerializeField] protected bool _enableOffscreenArrow = true;
    [Space(10)] [SerializeField] protected List<RaycastEntity> _entities;

    protected List<RaycastEntity> _completeInterractedObj = new();

    protected override void OnStartInteract(InteractableSignal signal)
    {
        if (_entities.Contains(signal.Value))
        {
            if (!_completeInterractedObj.Contains(signal.Value))
                InterractWithRightEntity(signal.Value);
            else
                FireSoftWrong(_softWrongMessages[0]);
        }
        else
            FireHardWrong();  
    }

    protected virtual void InterractWithRightEntity(RaycastEntity entity)
    {
        _completeInterractedObj.Add(entity);
        entity.Interactable.CanInterract = false;
        _higlightHolder.DisableHiglightFor(entity);

        if (CheckAllCompleted())
            FireCompleteSignal();
    }

    protected bool CheckAllCompleted()
    {
        foreach (var i in _entities)
        {
            if (!_completeInterractedObj.Contains(i))
                return false;
        }

        return true;
    }

    public override void HiglightEnable()
    {
        foreach (var i in _entities)
        {
            if (_enableOutline)
                _higlightHolder.EnableOultines(i.Outline);
            if (_enableOffscreenArrow)
                _higlightHolder.AddOffscreenTarget(i);
        }
    }

    public override void HiglightDisable()
    {
        _higlightHolder.DisableAll();
    }
}