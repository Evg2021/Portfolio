using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolBagPanelAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _panel;
    public void Open()
    {
        _panel.gameObject.SetActive(true);
        
        var startPosition = Pointer.current.position.ReadValue();
        var endPos = _panel.position;
        
        _panel.position = startPosition;
        _panel.localScale = Vector2.zero;
        
        _panel.DOMove(endPos, 0.2f).SetEase(Ease.Linear);
        _panel.DOScale(Vector2.one, 0.2f).SetEase(Ease.Linear);
    }

    public void Close()
    {
        _panel.gameObject.SetActive(false);
    }
}