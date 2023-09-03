using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public ControllerBase mainController;
    public ControllerBase secondController;

    private List<GameObject> outlines;
    private MeshRenderer[] Renderers;
    private static Material currentHighlightMaterial;

    public bool IsActive { get; private set; }
    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        IsActive = false;
        InitializeControllers();
        InitializeRenderers();
        IsInitialized = true;
    }

    protected virtual void InitializeControllers()
    {
        if (mainController)
        {
            mainController.Initialize();
            if (!mainController.isEnabled)
            {
                mainController = null;
            }
        }

        if (secondController)
        {
            secondController.Initialize();
            if (!secondController.isEnabled)
            {
                secondController = null;
            }
        }
    }

    public virtual void InitializeRenderers(bool dirty = false)
    {
        if ((mainController && mainController.isEnabled) || (secondController && secondController.isEnabled) || dirty)
        {
            Renderers = GetComponentsInChildren<MeshRenderer>();

            if (dirty)
            {
                currentHighlightMaterial = GlobalVariables.InteractableObjectHighlightMaterial;
            }
            else
            {
                currentHighlightMaterial = GlobalVariables.OutlineHighlightMaterial;
            }
        }
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

    public virtual PrimitiveObject[] GetPrimitives()
    {
        var allControllers = GetComponentsInChildren<ControllerBase>();

        if (allControllers != null && allControllers.Length > 0)
        {
            var result = new PrimitiveObject[allControllers.Length];
            for(int i = 0; i < allControllers.Length; i++)
            {
                result[i] = BindingManager.GetPrimitiveType(allControllers[i].transform);
            }

            return result;
        }

        return null;
    }

    public IController GetMainController()
    {
        return IsInitialized ? mainController : null;
    }

    public IController GetSecondController()
    {
        return IsInitialized ? secondController : null;
    }
}

public struct MeshColorMapItem
{
    public MeshRenderer Renderer;
    public Material OriginMaterial;
}