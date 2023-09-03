using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPanelConrtoller : MonoBehaviour
{
    public ToolPanelItemController[] Items;

    private Inventory inventory;

    private int localCurrentMaskIndex = -1;
    private int localCurrentHelmetIndex = -1;

    public void Initialize(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void OnEnable()
    {
        if (inventory)
        {
            SyncItems(inventory.Items);
        }
    }

    private void SyncItems(InventoryItem[] inventoryItems)
    {
        if (Items == null || Items.Length == 0)
        {
            Items = GetComponentsInChildren<ToolPanelItemController>();
        }

        if (Items != null && Items.Length > 0)
        {
            for (int i = 0; i < Items.Length && i < inventoryItems.Length; i++)
            {
                Items[i].SetIcon(inventoryItems[i].Icon);
            }

            ChangeCurrentSelectedTools(inventory.currentMaskIndex.Value, inventory.currentHelmetIndex.Value);
        }
    }

    private void ChangeCurrentSelectedTools(int mask, int helmet)
    {
        if (localCurrentMaskIndex != -1 && Items.Length > localCurrentMaskIndex)
        {
            Items[localCurrentMaskIndex].UnselectItem();
        }
        if (localCurrentHelmetIndex != -1 && Items.Length > localCurrentHelmetIndex)
        {
            Items[localCurrentHelmetIndex].UnselectItem();
        }

        localCurrentMaskIndex = mask;
        localCurrentHelmetIndex = helmet;

        if (localCurrentMaskIndex != -1 && Items.Length > localCurrentMaskIndex)
        {
            Items[localCurrentMaskIndex].SelectItem();
        }
        if (localCurrentHelmetIndex != -1 && Items.Length > localCurrentHelmetIndex)
        {
            Items[localCurrentHelmetIndex].SelectItem();
        }
    }

    private void Update()
    {
        if (inventory && Items != null && Items.Length > 0)
        {
            int currentMask = inventory.currentMaskIndex.Value;
            int currentHelmet = inventory.currentHelmetIndex.Value;

            if (currentMask != localCurrentMaskIndex || currentHelmet != localCurrentHelmetIndex)
            {
                ChangeCurrentSelectedTools(currentMask, currentHelmet);
            }
        }
    }
}
