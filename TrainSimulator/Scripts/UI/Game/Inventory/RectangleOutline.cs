using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using DG.Tweening;
using Zenject;

[RequireComponent(typeof(Rectangle))]
public class RectangleOutline : MonoBehaviour
{
    private ItemSlot _itemSlot;
    private Rectangle _outline;
    private GeoUtils.OutlineShapeProperties _outlineSettings;
    private bool _isSelected;
    private Tweener _flickeringTween;
    private InventorySettings _inventorySettings;
    
    public void Init(ItemSlot itemSlot, InventorySettings inventorySettings)
    {
        _inventorySettings = inventorySettings;
        _itemSlot = itemSlot;
        _outline = GetComponent<Rectangle>();

        _itemSlot.OnEnter.AddListener(MakeOutlineHighlighted);
        _itemSlot.OnExit.AddListener(MakeOutlineDeactive);
        _itemSlot.OnClick.AddListener(MakeOutlineSelected);

        SetupOutline();
    }

    private void OnDestroy()
    {
        _itemSlot.OnEnter.RemoveListener(MakeOutlineHighlighted);
        _itemSlot.OnExit.RemoveListener(MakeOutlineDeactive);
        _itemSlot.OnClick.RemoveListener(MakeOutlineSelected);
    }

    private void SetupOutline()
    {
        _outlineSettings = new GeoUtils.OutlineShapeProperties
        {
            DrawOutline = false,
            DrawOutlineShadow = false,
            DrawFill = true,
            DrawFillShadow = false,
            OutlineColor = _inventorySettings.NormalColor,
            FillColor = _inventorySettings.NormalColor
        };

        _outline.ShapeProperties = _outlineSettings;
    }

    private void MakeOutlineSelected(IItemSlot slot)
    {
        _isSelected = true;
        _outlineSettings.DrawOutline = true;
        _outlineSettings.OutlineColor = _inventorySettings.SelectedColor;
        RefreshDraw();
    }

    private void MakeOutlineHighlighted(IItemSlot slot)
    {
        if (_isSelected)
            return;

        _outlineSettings.DrawOutline = true;
        _outlineSettings.OutlineColor = _inventorySettings.HiglightColor;
        RefreshDraw();
    }

    private void MakeOutlineDeactive(IItemSlot slot)
    {
        if (_isSelected)
            return;

        MakeOutlineHide();
    }

    public void MakeOutlineHide()
    {
        _isSelected = false;
        _outlineSettings.DrawOutline = false;
        RefreshDraw();
    }

    private void RefreshDraw()
    {
        _outline.SetAllDirty();
    }

    public void Flickering()
    {
        _flickeringTween = DOVirtual.Color(_inventorySettings.FlickeringColor1, _inventorySettings.FlickeringColor2, 1,
            (x) =>
            {
                _outlineSettings.FillColor = x;
                RefreshDraw();
            }).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetLink(gameObject);
    }

    public void StopFlickering()
    {
        _flickeringTween?.Kill();
        _outlineSettings.FillColor = _inventorySettings.NormalColor;
    }
}