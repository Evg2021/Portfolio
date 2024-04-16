using System.Collections;
using UnityEngine;
using Zenject;

public class TaskInterractWithItem : TaskInteract
{
    [SerializeField] private ItemData _requieredItem;
    [SerializeField] private float _usageTime;

    [Inject] protected RadialCheckBar _radialCheckBar;
    private IEnumerator _lookCoroitine;

    private class EntityContainer
    {
        public RaycastEntity Entity;
        public IItemInteractable ItemInteractable;
    }

    private HandHeldObjectSpawner _handHeldObjectSpawner;

    [Inject]
    protected virtual void Construct(HandHeldObjectSpawner handHeldObjectSpawner)
    {
        _handHeldObjectSpawner = handHeldObjectSpawner;
    }

    protected override void InterractWithRightEntity(RaycastEntity entity)
    {
        if (_inventoryPanel.GetCurrentItem() == _requieredItem || _requieredItem == null)
        {
            if (entity.Interactable.TryGetComponent(out IItemInteractable itemInteractable))
            {
                EntityContainer entityContainer = new EntityContainer()
                {
                    Entity = entity,
                    ItemInteractable = itemInteractable
                };
                
                _lookCoroitine = StartRadialBar(entityContainer);
                StartCoroutine(_lookCoroitine);
            }
            else
            {
                Debug.LogError(entity.name + " - Current entity not have IItemInteract interface");
            }
        }
        else
        {
            FireSoftWrong(MessageWrongItem());
        }
    }

    private IEnumerator StartRadialBar(EntityContainer entityContainer)
    {
        _radialCheckBar.Launch(_usageTime);
        yield return new WaitForSeconds(_usageTime);
        _radialCheckBar.Reset();
        
        entityContainer.ItemInteractable.InteractWithItem();

        if (_handHeldObjectSpawner.CurrentHandHeld != null)
            LaunchHandHeldAnimation();

        _completeInterractedObj.Add(entityContainer.Entity);
        entityContainer.Entity.Interactable.CanInterract = false;
        _higlightHolder.DisableHiglightFor(entityContainer.Entity);
        
        if (CheckAllCompleted())
            FireCompleteSignal();
    }

    private void LaunchHandHeldAnimation()
    {
        if (_handHeldObjectSpawner.CurrentHandHeld.TryGetComponent(out HandHeldInteractable handHeld))
            handHeld.PlayAnimation();
    }

    protected override void OnStopInterract(InteractableSignal signal)
    {
        base.OnStopInterract(signal);
        if (_lookCoroitine != null)
            StopCoroutine(_lookCoroitine);
        _radialCheckBar.Reset();
    }

    private string MessageWrongItem()
    {
        if (_softWrongMessages.Count == 0)
        {
            Debug.LogError("Soft wrong message is missing!!!");
            _softWrongMessages.Add("Soft wrong message is missing!");
        }

        return _softWrongMessages[0];
    }
}