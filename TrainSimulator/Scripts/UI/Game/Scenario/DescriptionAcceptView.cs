using UnityEngine;
using UnityEngine.UI;

public class DescriptionAcceptView : InfoMessageView
{
    public delegate void ClickAcceptButton();
    public ClickAcceptButton OnClick = delegate { };

    [SerializeField] private Button _acceptButton;

    protected override void Awake()
    {
        _acceptButton.onClick.AddListener(OnAcceptClick);
        base.Awake();
    }

    protected void OnDestroy()
    {
        _acceptButton.onClick.RemoveListener(OnAcceptClick);
    }

    protected void OnAcceptClick()
    {
        OnClick?.Invoke();
    }
}
