using DG.Tweening;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ScaledButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scaling")]
    [SerializeField] private bool _lockScaling = true;
    [SerializeField] private float _scaleDepth = 1.1f;

    [Header("Outline settings")]
    [SerializeField] private GeoUtils.OutlineShapeProperties _outlineSettings;

    [Header("Events")]
    [SerializeField] private UnityEvent _onClick;

    private RectTransform _rect;
    private Rectangle _rectangle;

    private Vector3 _startScale;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _rectangle = GetComponent<Rectangle>();

        _startScale = _rect.localScale;
        _rectangle.ShapeProperties = _outlineSettings;

        ShowShadow(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_lockScaling)
        {
            _rect.SetAsLastSibling();
            //_rect.localScale = _rect.localScale * _scaleDepth;
            _rect.DOScale(_startScale * _scaleDepth, 0.05f).SetEase(Ease.Linear);
        }

        ShowShadow(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_lockScaling)
            _rect.DOScale(_startScale, 0.05f).SetEase(Ease.Linear);
            //_rect.localScale = _startScale;

        ShowShadow(false);
    }

    private void ShowShadow(bool value)
    {
        _rectangle.ShadowProperties.ShowShadows = value;
        RefreshDraw();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke();

        if (!_lockScaling)
            _rect.localScale = _startScale;
    }
    
    private void RefreshDraw()
    {
        _rectangle.SetAllDirty();
    }

}
