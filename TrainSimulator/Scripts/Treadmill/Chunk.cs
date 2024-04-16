using UnityEngine;

public class Chunk : MonoBehaviour
{
    public string Name => _name;
    public float Size => _size;

    [SerializeField] private string _name;
    [SerializeField] private float _size;

    private void OnValidate()
    {
        if (_name.Length == 0)
            _name = gameObject.name;
    }
}