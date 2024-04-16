using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskPlaceItemToPoint : TaskInteract
{
    [SerializeField] private List<RaycastEntity> _wrongPoints;
    [SerializeField] private ItemData _interractedItem;

    protected override void LaunchTask()
    {
        base.LaunchTask();
        foreach (var i in _wrongPoints)
            i.Interactable.CanInterract = true;
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        RaycastEntity entity = signal.Value;

        if (entity.TryGetComponent(out HologramInterract hologrammPoint))
        {
            if (_entities.Contains(entity))
            {
                if (_inventoryPanel.GetCurrentItem() == _interractedItem)
                {
                    hologrammPoint.Change();
                    _higlightHolder.DisableHiglightFor(entity);
                    _inventoryPanel.DeleteCurrentItem();

                    if (IsLastPlacePoint())
                    {
                        HideAllWrong();
                        FireCompleteSignal();
                    }
                }
                else
                {
                    //Взаимодействуем не тем предметом
                    FireSoftWrong(_softWrongMessages[0]);
                }
            }
            else
            {
                //Взаимодействие с ошибочной точкой
                FireHardWrong();
            }
        }
        else if (InterractWithHiddenObject(entity))
        {
            //Если уже с точкой взаимодействовали
            FireSoftWrong(_softWrongMessages[1]);
        }
        else
        {
            FireHardWrong();
        }
    }

    public bool InterractWithHiddenObject(RaycastEntity entity)
    {
        foreach (var i in _entities)
        {
            if (i.GetComponent<HologramInterract>().HiddenObject == entity)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsLastPlacePoint()
    {
        foreach (var i in _entities)
        {
            if (!i.GetComponent<HologramInterract>().IsPlaced)
            {
                return false;
            }
        }

        return true;
    }

    private void HideAllWrong()
    {
        foreach (var i in _wrongPoints)
        {
            i.gameObject.SetActive(false);
        }
    }

    public override void HiglightEnable()
    {
        base.HiglightEnable();
        foreach (var i in _wrongPoints)
        {
            _higlightHolder.EnableOultines(i.Outline);
            _higlightHolder.AddOffscreenTarget(i);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _higlightHolder.DisableAll();
    }
}