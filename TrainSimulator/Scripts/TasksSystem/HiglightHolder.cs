using System.Collections.Generic;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;
using Object = UnityEngine.Object;

public class HiglightHolder : IInitializable
{
    private readonly List<Outline> _outlines = new();
    private List<OffscreenTarget> _offscreenTargets = new();
    private SignalBus _signalBus;

    public void Initialize()
    {
        if (Object.FindObjectsOfType(typeof(Outline)) is Outline[] outlines)
            _outlines.AddRange(outlines);
    }

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void EnableOultines(List<Outline> list)
    {
        _outlines.AddRange(list);

        foreach (var i in list)
            i.enabled = true;
    }

    public void EnableOultines(Outline outline)
    {
        _outlines.Add(outline);
        outline.enabled = true;
    }


    public void DisableHiglightFor(RaycastEntity raycastEntity)
    {
        if (raycastEntity.Outline != null)
            raycastEntity.Outline.enabled = false;

        if (raycastEntity.TryGetComponent(out OffscreenTarget offscreenTarget))
        {
            Object.Destroy(offscreenTarget);
        }
    }
    public void DisableAll()
    {
        foreach (var i in _outlines)
            i.enabled = false;

        _outlines.Clear();
        ClearOffscreenTargets();
    }

    public void AddOffscreenTarget(RaycastEntity raycastEntity)
    {
        OffscreenTarget offscreenTarget = raycastEntity.gameObject.AddComponent<OffscreenTarget>();
        TargetVisualInfo targetVisualInfo = new TargetVisualInfo()
        {
            OffscreenOffset = raycastEntity.OffscreenTargetOffset,
            Sprite = raycastEntity.Description.Sprite
        };
        offscreenTarget.Init(_signalBus, targetVisualInfo);
        _offscreenTargets.Add(offscreenTarget);
    }
    public void AddOffscreenTargetWithoutEntity(GameObject targetGameObject, TargetVisualInfo targetVisualInfo)
    {
        OffscreenTarget offscreenTarget = targetGameObject.AddComponent<OffscreenTarget>();
        offscreenTarget.Init(_signalBus, targetVisualInfo);
        _offscreenTargets.Add(offscreenTarget);
    }
    private void ClearOffscreenTargets()
    {
        foreach (var i in _offscreenTargets)
        {
            Object.Destroy(i);
        }
    }
}