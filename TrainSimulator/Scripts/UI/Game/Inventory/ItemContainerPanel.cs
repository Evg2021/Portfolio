using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ItemContainerPanel : MonoBehaviour
{
    [SerializeField] private ToolBagPanelAnimation _toolBagPanelAnimation;
    [SerializeField] private GameObject _panelView;
    [SerializeField] private Transform _itemRoot;
    [SerializeField] private SlotComponentContainer _slotComponentContainerPrefab;
    [SerializeField] private InventorySettings _inventorySettings;

    private SignalBus _signalBus;
    private ItemContainer _lastItemContainer;

    public List<ISlotComponentContainer> Slots { get; } = new();
    public UnityEvent<ISlotComponentContainer> OnItemRemove { get; } = new();
    public UnityEvent<ISlotComponentContainer> OnItemReceive { get; } = new();

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<PlayerStateChangeSignal>(OnStateChange);
        _signalBus.Subscribe<StartInteractSignal>(InteractWith);

        InitializeSlots();
    }

    private void InteractWith(StartInteractSignal obj)
    {
        if (_lastItemContainer != obj.Value.Interactable)
            _toolBagPanelAnimation.Close();
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<PlayerStateChangeSignal>(OnStateChange);
        _signalBus.TryUnsubscribe<StartInteractSignal>(InteractWith);
    }

    private void OnStateChange(PlayerStateChangeSignal obj)
    {
        if (obj.Value == PlayerStates.Moving)
            HidePanel();
    }

    private void InitializeSlots()
    {
        Slots.AddRange(_panelView.GetComponentsInChildren<ISlotComponentContainer>());
        foreach (var slot in Slots)
        {
            InitSlot(slot);
        }
    }

    private void InitSlot(ISlotComponentContainer slot)
    {
        slot.Initialize(_inventorySettings);
    }

    private void OnSlotClick(ISlotComponentContainer slot)
    {
        slot.OnClickSlot.RemoveListener(OnSlotClick);
        Slots.Remove(slot);
        _lastItemContainer.Remove(slot);

        OnItemRemove?.Invoke(slot);
    }

    public void ShowPanel(List<ItemData> itemsContain, ItemContainer itemContainer)
    {
        _toolBagPanelAnimation.Open();

        if (_lastItemContainer != itemContainer)
        {
            _lastItemContainer = itemContainer;
            RefreshPanel();
            CreateSlots(itemsContain);
        }
    }

    private void RefreshPanel()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            Slots[i].Destroy();
        }

        Slots.Clear();
    }

    private void CreateSlots(List<ItemData> itemsContain)
    {
        foreach (var item in itemsContain)
        {
            SpawnSlot(item);
        }
    }

    private void SpawnSlot(ItemData itemData)
    {
        ISlotComponentContainer slot = Instantiate(_slotComponentContainerPrefab, _itemRoot);
        InitSlot(slot);
        slot.ItemSlot.StoredItemData = itemData;
        Slots.Add(slot);

        slot.OnClickSlot.AddListener(OnSlotClick);
        OnItemReceive?.Invoke(slot);
    }

    public void HidePanel()
    {
        _toolBagPanelAnimation.Close();
    }
}