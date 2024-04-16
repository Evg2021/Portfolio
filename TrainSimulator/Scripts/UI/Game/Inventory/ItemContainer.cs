using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ItemContainer : Interactable
{
    [Space(20)] [Header("Item container")] [SerializeField]
    protected List<ItemData> _itemsContain;

    protected ItemContainerPanel _itemContainerPanel;

    [Inject]
    private void Construct(ItemContainerPanel itemContainerPanel)
    {
        _itemContainerPanel = itemContainerPanel;
    }

    public override void StartInteract()
    {
        base.StartInteract();
        if (CanInterract)
            _itemContainerPanel.ShowPanel(_itemsContain, this);
    }

    public void Remove(ISlotComponentContainer slot)
    {
        _itemsContain.Remove(slot.ItemSlot.StoredItemData);
    }

    public void HighlightItems(List<ItemData> importantItemList)
    {
        var slotsWithCorrectItems =
            _itemContainerPanel.Slots.Where(x => importantItemList.Contains(x.ItemSlot.StoredItemData));
        foreach (ISlotComponentContainer i in slotsWithCorrectItems)
        {
            i.RectangleOutline.Flickering();
        }
    }
}