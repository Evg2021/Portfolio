using UnityEngine;

[CreateAssetMenu(fileName = "PrimitiveObject", menuName = "Create Primitive Object")]
public class PrimitiveObject : ScriptableObject
{
    public string Name;
    public string KeyName;
    public string ParentKeyName;
    public string TypeControllerName;
    public string SetParameterName;
    public string GetParameterName;
}
