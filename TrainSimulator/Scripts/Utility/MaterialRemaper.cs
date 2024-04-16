using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialRemaper : MonoBehaviour
{
    public GameObject sourceObject;
    public GameObject targetObject;
    [ContextMenu("Remap")]
    public void TransferMaterials()
    {
        Transform[] sources = sourceObject.GetComponentsInChildren<Transform>();
        Transform[] targets = targetObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < sources.Length; i++)
        {
            var currentTransform = sources[i];
            MeshRenderer currentMR;
            if(currentTransform.TryGetComponent(out MeshRenderer mr))
            {
                currentMR = mr;
            }
            else
            {
                continue;
            }
            
            for (int j = 0; j < targets.Length; j++)
            {
                if (currentTransform.name == targets[j].name)
                {
                    Material[] materials = currentMR.sharedMaterials;
                    targets[j].GetComponent<MeshRenderer>().sharedMaterials = materials;
                    break;
                }
            }
        }
        //
        // for (int i = 0; i < sources.Length; i++)
        // {
        //     Material[] materials = sources[i].GetComponent<MeshRenderer>().sharedMaterials;
        //     targets[i].GetComponent<MeshRenderer>().sharedMaterials = materials;
        // }
    }
}
