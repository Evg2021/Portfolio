using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


public interface IItemSlot
{
    public ItemData StoredItemData { get; set; }
    public void BlockedInterract(bool value);
}

public class ItemSlot : MonoBehaviour, IItemSlot, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    [SerializeField] private Image _imageHolder;
    [SerializeField] private ItemData _storedItemData;

    public ItemData StoredItemData
    {
        get => _storedItemData;
        set
        {
            _storedItemData = value;
            SetItem(_storedItemData);
        }
    }

    public UnityEvent<IItemSlot> OnEnter { get; } = new();
    public UnityEvent<IItemSlot> OnExit { get; } = new();
    public UnityEvent<IItemSlot> OnClick { get; } = new();

    private bool _isBlocked = false;

    public void BlockedInterract(bool value)
    {
        _isBlocked = value;
    }

    private void OnValidate()
    {
        if (_storedItemData != null)
        {
            transform.name = _storedItemData.Name;
            
            if (_imageHolder == null)
            {
                transform.GetChild(0).TryGetComponent(out Image image);
                {
                    _imageHolder = image;
                }
            }

            SetItem(_storedItemData);
        }
    }

    private void SetItem(ItemData newItemData)
    {
        _storedItemData = newItemData;
        _imageHolder.sprite = newItemData.Sprite;
        transform.name = newItemData.Name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("EnterCursorToSLot");
        OnEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("ExitCursorToSLot");
        OnExit?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isBlocked)
        {
            print("Button is blocked");
            return;
        }
        
        print("ClickCursorToSLot");
        OnClick?.Invoke(this);
    }
}