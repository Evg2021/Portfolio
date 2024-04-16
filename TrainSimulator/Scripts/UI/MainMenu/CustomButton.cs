using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _pressingDepth = 6f;
    [SerializeField] private RectTransform _selfRTransform;

    private void OnValidate()
    {
        _selfRTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData pointerData)
    {
        _selfRTransform.Translate(Vector2.down * _pressingDepth);
    }

    public void OnPointerUp(PointerEventData pointerData)
    {
        _selfRTransform.Translate(Vector2.up * _pressingDepth);
    }
}
