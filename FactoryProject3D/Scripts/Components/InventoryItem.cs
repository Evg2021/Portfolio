using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public bool IsMask;
    public Sprite Icon;
    private new SkinnedMeshRenderer renderer;

    private void SetActiveItem(bool value)
    {
        gameObject.SetActive(value);
    }
    public void HideItem()
    {
        SetActiveItem(false);
    }
    public void ShowItem()
    {
        SetActiveItem(true);
    }

    private void InitializeSkinnedMeshRenderer(Transform root)
    {
        if (!renderer)
        {
            renderer = GetComponent<SkinnedMeshRenderer>();
        }

        if (renderer)
        {
            renderer.rootBone = root;
        }
    }
}
