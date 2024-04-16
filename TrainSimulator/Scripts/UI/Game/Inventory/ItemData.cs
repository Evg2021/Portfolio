using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public EItemType ItemType;
    public Sprite Sprite;
    public string Name;
    public string Description;
    public RaycastEntity HandHeldObjPrefab;
    public Vector3 HandHeldRotation;
}