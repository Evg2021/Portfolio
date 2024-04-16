using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InventorySettings", menuName = "Data/InventorySettings")]
public class InventorySettings : ScriptableObject
{
    public Color32 HiglightColor;
    public Color32 SelectedColor;
    public Color32 NormalColor;
    
    [Space(10)]
    public Color32 FlickeringColor1;
    public Color32 FlickeringColor2;
}