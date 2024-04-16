using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoMessageView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Image _panel;
    [SerializeField] protected TextMeshProUGUI _headerText;
    [SerializeField] protected TextMeshProUGUI _contentText;

    protected virtual void Awake()
    {
        SetActive(false);
    }

    public virtual void Show(InfoMessage message)
    {
        if (message.Name.Length + message.Content.Length == 0) return;

        SetActive(true);
        _headerText.text = message.Name;
        _contentText.text = message.Content;
    }

    public virtual void Hide()
    {
        SetActive(false);
    }

    protected virtual void SetActive(bool value)
    {
        _panel.gameObject.SetActive(value);
    }
}
