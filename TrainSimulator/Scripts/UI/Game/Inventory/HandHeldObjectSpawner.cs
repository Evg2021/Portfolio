using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HandHeldObjectSpawner : MonoBehaviour
{
    private SignalBus _signalBus;
    private Dictionary<RaycastEntity, RaycastEntity> _tools = new(); //prefab, game object
    private RaycastEntity _currentHandHeld;
    private HandHeldPoint _handHeldPoint;
    private GameplaySettings _gameplaySettings;
    public RaycastEntity CurrentHandHeld => _currentHandHeld;

    public RaycastEntity GetHandHeldByPrefab(RaycastEntity prefab)
    {
        if (_tools.TryGetValue(prefab, out RaycastEntity obj))
            return obj;

        return null;
    }

    public bool IsContainEntity(RaycastEntity obj)
    {
        var contain = _tools.ContainsValue(obj);
        return contain;
    }

    [Inject]
    private void Construct(SignalBus signalBus, HandHeldPoint handHeldPoint, GameplaySettings gameplaySettings)
    {
        _gameplaySettings = gameplaySettings;
        _handHeldPoint = handHeldPoint;
        _signalBus = signalBus;

        _signalBus.Subscribe<ClickToolSlotSignal>(SpawnHandHeldObject);
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ClickToolSlotSignal>(SpawnHandHeldObject);
    }

    private void SpawnHandHeldObject(ClickToolSlotSignal obj)
    {
        ItemData item = obj.Value.ItemSlot.StoredItemData;
        RaycastEntity prefab = item.HandHeldObjPrefab;

        HideCurrentHandHeld();

        if (prefab != null)
        {
            if (!_tools.ContainsKey(prefab))
            {
                _currentHandHeld = Spawn(item.HandHeldRotation, prefab);
                _tools.Add(prefab, _currentHandHeld);
            }
            else
            {
                if (_tools.TryGetValue(prefab, out RaycastEntity tool))
                {
                    tool.gameObject.SetActive(true);
                    _currentHandHeld = tool;
                }
            }
        }
        else
        {
            print("Item has no prefab to show");
        }
    }

    private void HideCurrentHandHeld()
    {
        if (_currentHandHeld != null)
            _currentHandHeld.gameObject.SetActive(false);
    }

    private RaycastEntity Spawn(Vector3 rotation, RaycastEntity prefab)
    {
        RaycastEntity handHeldObject = Instantiate(prefab, _handHeldPoint.transform);
        handHeldObject.Outline.SetSettings(_gameplaySettings);
        handHeldObject.transform.localPosition = Vector3.zero;
        handHeldObject.transform.localRotation = Quaternion.Euler(rotation);
        
        return handHeldObject;
    }
}