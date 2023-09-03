using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenObjectControllerBase : MonoBehaviour
{
    public string ParameterName;
    public Types DataType;

    public uint Index;

    private MeshRenderer meshRenderer;
    private Material outlineMaterial;

    protected bool isActive = false;

    unsafe protected void* get;
    unsafe protected void* set;

    unsafe public virtual void Initialize(uint index, void* get, void* set)
    {
        Index = index;
        this.get = get;
        this.set = set;

        InitializeRenderer();
        InitializeMaterial();
    }
    private void InitializeRenderer()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void InitializeMaterial()
    {
        outlineMaterial = Resources.Load<Material>("Materials\\OutlineMaterial");
    }

    public virtual void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            EnableHighlight();
        }
    }
    public virtual void Disactivate()
    {
        if (isActive)
        {
            isActive = false;
            DisableHighlight();
        }
    }

    public void EnableHighlight()
    {
        if (meshRenderer && outlineMaterial)
        {
            var oldMaterials = meshRenderer.materials;
            var newMaterials = new Material[oldMaterials.Length + 1];
            for (int i = 0; i < oldMaterials.Length; i++)
            {
                newMaterials[i] = oldMaterials[i];
            }
            newMaterials[oldMaterials.Length] = outlineMaterial;
            meshRenderer.materials = newMaterials;
        }
    }
    public void DisableHighlight()
    {
        if (meshRenderer && outlineMaterial)
        {
            var newMaterials = new List<Material>();
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                if (!meshRenderer.materials[i].name.Contains(outlineMaterial.name))
                {
                    newMaterials.Add(meshRenderer.materials[i]);
                }
            }
            meshRenderer.materials = newMaterials.ToArray();
        }
    }
}
