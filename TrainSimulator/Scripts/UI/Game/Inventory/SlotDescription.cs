using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using Zenject;

public class SlotDescription : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _textDescription;
    [SerializeField] private RectTransform _panel;
    
    private List<ISlotComponentContainer> _slots = new();
    private InventoryPanel _inventoryPanel;
    private ItemContainerPanel _itemContainerPanel;
    private SignalBus _signalBus;
    private Vector3 _panelStartPos;
    
    [Inject]
    private void Construct(InventoryPanel inventoryPanel, ItemContainerPanel itemContainerPanel, SignalBus signalBus)
    {
        _signalBus = signalBus;
        _itemContainerPanel = itemContainerPanel;
        _inventoryPanel = inventoryPanel;
        
        CollectAllSlots();
        Subscribe();
        
        _panel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var slot in _slots)
        {
            slot.OnEnterSlot.RemoveListener(OnSlotEnter);
            slot.OnExitSlot.RemoveListener(OnSlotExit);
        }
        
        _inventoryPanel.OnItemReceive.RemoveListener(AddToList);
        _inventoryPanel.OnItemDelete.RemoveListener(RemoveFromList);
        _itemContainerPanel.OnItemReceive.RemoveListener(AddToList);
        _signalBus.Unsubscribe<PlayerStateChangeSignal>(OnChangePlayerState);
    }


    private void Subscribe()
    {
        foreach (var slot in _slots)
        {
            slot.OnEnterSlot.AddListener(OnSlotEnter);
            slot.OnExitSlot.AddListener(OnSlotExit);
        }

        _inventoryPanel.OnItemReceive.AddListener(AddToList);
        _inventoryPanel.OnItemDelete.AddListener(RemoveFromList);
        _itemContainerPanel.OnItemReceive.AddListener(AddToList);
        _signalBus.Subscribe<PlayerStateChangeSignal>(OnChangePlayerState);
    }


    private void OnChangePlayerState(PlayerStateChangeSignal obj)
    {
        if(obj.Value == PlayerStates.Moving)
            _panel.gameObject.SetActive(false);
    }

    private void CollectAllSlots()
    {
        var inventorySlots = _inventoryPanel.Slots.Select(x => x);
        var toolBagSlots = _itemContainerPanel.Slots.Select(x => x);
        _slots.AddRange(inventorySlots);
        _slots.AddRange(toolBagSlots);
    }

    private void AddToList(ISlotComponentContainer slot)
    {
        if(_slots.Contains(slot))
            return;
        
        slot.OnEnterSlot.AddListener(OnSlotEnter);
        slot.OnExitSlot.AddListener(OnSlotExit);
        _slots.Add(slot);
    }
    
    private void RemoveFromList(ISlotComponentContainer slot)
    {
        slot.OnEnterSlot.RemoveListener(OnSlotEnter);
        slot.OnExitSlot.RemoveListener(OnSlotExit);
        _slots.Remove(slot);
        _panel.gameObject.SetActive(false);
    }

    private void OnSlotExit(ISlotComponentContainer slot)
    {
        _panel.gameObject.SetActive(false);
        _panel.localPosition = Vector3.zero;
    }

    private void OnSlotEnter(ISlotComponentContainer slot)
    {
        SetText(slot.ItemSlot.StoredItemData);
        _panel.gameObject.SetActive(true);
        SetPosition(slot.TransformSelf);
        StartCoroutine(CheckElementContainInScreen(_panel));
    }

    private void SetPosition(Transform slotTransform)
    {
        var oldParent = _rectTransform.parent;
        _rectTransform.parent = slotTransform.transform;
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.parent = oldParent;
    }

    private void SetText(ItemData itemData)
    {
        _textDescription.text = itemData.Name;
    }
    private IEnumerator CheckElementContainInScreen(RectTransform uiElement)
    {
        yield return null;
        Vector3[] corners = new Vector3[4];
        uiElement.GetWorldCorners(corners);

        Vector3 position = uiElement.position;

        float minX = corners[0].x;
        // float maxX = corners[3].x;
        // float minY = corners[0].y;
        // float maxY = corners[1].y;

        // float screenWidth = Screen.width;
        // float screenHeight = Screen.height;

        if (minX < 0)
        {
            position.x -= minX;
            uiElement.position = position;
        }
        else
        {
            uiElement.localPosition = Vector3.zero;
        }
        // else if (maxX > screenWidth)
        // {
        //     position.x -= maxX - screenWidth;
        // }
        //
        // if (minY < 0)
        // {
        //     position.y -= minY;
        // }
        // else if (maxY > screenHeight)
        // {
        //     position.y -= maxY - screenHeight;
        // }
    }
}