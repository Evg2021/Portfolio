using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class EnvironmentControllerBase : MonoBehaviour, IInteractable
{
    private const string interactableLayerName = "Interactable";

    public bool IsActive { get; protected set; }
    public uint Index { get; private set; }

    private List<GameObject> outlines;
    public MeshRenderer[] Renderers;
    private static Material currentHighlightMaterial;

    public bool CurrentValue { get; protected set; }

    public virtual void Initialize(uint index)
    {
        Index = index;
        InitializeRenderers();
        InitializeLayer();
        CurrentValue = false;
    }

    private void InitializeLayer()
    {
        var layer = LayerMask.NameToLayer(interactableLayerName);
        if (gameObject.layer != layer)
        {
            gameObject.layer = layer;   
        }
    }
    public virtual void InitializeRenderers()
    {
        Renderers = GetComponents<MeshRenderer>();

        if(currentHighlightMaterial == null)
        currentHighlightMaterial = GlobalVariables.EnvironmentOutlineHighlightMaterial;
    }

    public void DisableHighlight()
    {
        if (IsActive)
        {
            if (outlines != null && outlines.Count > 0)
            {
                foreach (var item in outlines)
                {
                    Destroy(item.gameObject);
                }
            }

            IsActive = false;
        }
    }

    public void EnableHighlight()
    {
        if (!IsActive)
        {
            if (Renderers != null && Renderers.Length > 0 && currentHighlightMaterial)
            {
                outlines = new List<GameObject>();
                foreach (var item in Renderers)
                {
                    outlines.Add(Utilities.GenerateOutline(item, currentHighlightMaterial));
                }
            }

            IsActive = true;
        }
    }

    public IController GetMainController()
    {
        return null;
    }

    public IController GetSecondController()
    {
        return null;
    }

    public abstract void Interact();
    public abstract void SetState(bool state);
}
