using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityFixer : MonoBehaviour
{
    [ContextMenu("Fix box by mesh")]
    private void FitColliderToMesh()
    {
        MeshRenderer meshRenderer = GetComponent<Outline>().RendererAditional;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        
        Bounds meshBounds = meshRenderer.bounds;
        boxCollider.size = meshBounds.size;
        boxCollider.center = transform.InverseTransformPoint(meshRenderer.gameObject.transform.position);
    }

    [ContextMenu("Fix offscreen position")]
    private void FixOffscreen()
    {
        RaycastEntity raycastEntity = GetComponent<RaycastEntity>();
        MeshRenderer meshRenderer = GetComponent<Outline>().RendererAditional;
        
        raycastEntity.OffscreenTargetOffset =
            transform.InverseTransformPoint(meshRenderer.gameObject.transform.position);
    }
}