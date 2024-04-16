using UnityEngine;

[RequireComponent(typeof(RaycastEntity))]
public class EntityDescription : MonoBehaviour
{
    public string Name{
        get
        {
            return _name;
        }
        set => _name = value;
    }

    public string Description
    {
        get
        {
            return _description;
        }
        set => _description = value;
    }
    public Sprite Sprite => _sprite;

    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected Sprite _sprite;
}
