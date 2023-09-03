using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesAsset", menuName = "Create Resources Asset")]
public class ResourcesAsset : SingletonAsset<ResourcesAsset>
{
    public Material SelectedMaterial;
    public Material IndicatorMaterial;
    public Material SelectedPipeMaterial;
    public Material IndicatorPipeMaterial;
}
