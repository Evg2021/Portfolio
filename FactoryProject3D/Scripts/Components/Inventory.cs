using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : NetworkBehaviour
{
    private const string canvasName = "Canvas";

    public Transform RootBone;
    public ToolPanelConrtoller ToolPanelPrefab;
    public InventoryItem[] Items;

    public NetworkVariable<int> currentMaskIndex = new NetworkVariable<int>(-1);
    public NetworkVariable<int> currentHelmetIndex = new NetworkVariable<int>(-1);

    private int localCurrentMaskIndex = -1;
    private int localCurrentHelmetIndex = -1;

    private StarterAssetsInputs input;

    private ToolPanelConrtoller toolPanelUI;

    private void Reset()
    {
        Items = GetComponentsInChildren<InventoryItem>();
    }

    private void OnValidate()
    {
        Items = GetComponentsInChildren<InventoryItem>();
    }

    private void Start()
    {
        InitializeItems();

        if (IsOwner)
        {
            InitialzeInput();
            InitializeToolPanelUI();
        }
    }
    private void InitializeItems()
    {
        if (Items != null && Items.Length > 0)
        {
            foreach (var item in Items)
            {
                item.HideItem();
            }
        }

        if (currentMaskIndex.Value != -1)
        {
            localCurrentMaskIndex = currentMaskIndex.Value;
            ShowItem(localCurrentMaskIndex);
        }

        if (currentHelmetIndex.Value != -1)
        {
            localCurrentHelmetIndex = currentHelmetIndex.Value;
            ShowItem(localCurrentHelmetIndex);
        }
    }
    private void InitialzeInput()
    {
        input = GetComponent<StarterAssetsInputs>();

        if (input)
        {
            input.OnInventoryPressed += Input_OnInventoryPressed;
            input.OnCallInventoryPressed += Input_OnCallInventoryPressed;
        }
    }

    private void InitializeToolPanelUI()
    {
        if (ToolPanelPrefab)
        {
            var canvas = GameObject.Find(canvasName);
            if (canvas)
            {
                toolPanelUI = Instantiate(ToolPanelPrefab, canvas.transform);
                toolPanelUI.Initialize(this);
                HideToolPanel();
            }
        }
    }

    private void Input_OnInventoryPressed(int index)
    {
        SetItem(index - 1);
    }
    private void Input_OnCallInventoryPressed(bool value)
    {
        if (value)
        {
            ShowToolPanel();
        }
        else
        {
            HideToolPanel();
        }
    }

    private void HideItem(int index)
    {
        if (Items != null && Items.Length > index && index >= 0)
        {
            Items[index].HideItem();
        }
    }
    private void ShowItem(int index)
    {
        if (Items != null && Items.Length > index && index >= 0)
        {
            Items[index].ShowItem();
        }
    }
    private void SetItem(int index)
    {
        if (Items != null && Items.Length > index && index >= 0)
        {
            if (index == localCurrentHelmetIndex)
            {
                HideItem(localCurrentHelmetIndex);
                localCurrentHelmetIndex = -1;
            }
            else if (index == localCurrentMaskIndex)
            {
                HideItem(localCurrentMaskIndex);
                localCurrentMaskIndex = -1;
            }
            else
            {
                var item = Items[index];
                if (item)
                {
                    if (item.IsMask)
                    {
                        HideItem(localCurrentMaskIndex);
                        localCurrentMaskIndex = index;
                    }
                    else
                    {
                        HideItem(localCurrentHelmetIndex);
                        localCurrentHelmetIndex = index;
                    }
                    
                }
            }

            if (currentHelmetIndex.Value != localCurrentHelmetIndex)
            {
                SyncCurrentItemServerRpc(localCurrentHelmetIndex, false);
                ShowItem(localCurrentHelmetIndex);
            }

            if (currentMaskIndex.Value != localCurrentMaskIndex)
            {
                SyncCurrentItemServerRpc(localCurrentMaskIndex, true);
                ShowItem(localCurrentMaskIndex);
            }
        }
    }

    [ServerRpc]
    private void SyncCurrentItemServerRpc(int index, bool isMask)
    {
        if (IsServer || IsHost)
        {
            if (isMask)
            {
                currentMaskIndex.Value = index;
            }
            else
            {
                currentHelmetIndex.Value = index;
            }

            SyncCurrentItemClientRpc(index, isMask);
        }
    }

    [ClientRpc]
    private void SyncCurrentItemClientRpc(int index, bool isMask)
    {
        if (IsClient && !IsOwner)
        {
            if (isMask)
            {
                HideItem(localCurrentMaskIndex);
                localCurrentMaskIndex = index;
                ShowItem(localCurrentMaskIndex);
            }
            else
            {
                HideItem(localCurrentHelmetIndex);
                localCurrentHelmetIndex = index;
                ShowItem(localCurrentHelmetIndex);
            }
        }
    }

    private void SetActiveToolPanel(bool value)
    {
        if (toolPanelUI)
        {
            toolPanelUI.gameObject.SetActive(value);
        }
    }
    public void HideToolPanel()
    {
        SetActiveToolPanel(false);
    }
    public void ShowToolPanel()
    {
        SetActiveToolPanel(true);
    }
}
