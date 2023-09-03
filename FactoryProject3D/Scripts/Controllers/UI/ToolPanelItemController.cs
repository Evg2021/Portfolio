using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolPanelItemController : MonoBehaviour
{
    private const string iconName = "Icon";
    private const float chosenIconAlpha = 1.0f;
    private const float nonChosenIconAlpha = 0.5f;

    private Image Icon;

    public void SetIcon(Sprite sprite)
    {
        if (!Icon)
        {
            InitializeIconComponent();
        }

        if (Icon)
        {
            Icon.sprite = sprite;
        }
    }

    private void InitializeIconComponent()
    {
        var iconObject = transform.Find(iconName);
        if (iconObject)
        {
            Icon = iconObject.GetComponent<Image>();
            UnselectItem();
        }
    }

    public void SelectItem()
    {
        if (Icon)
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, chosenIconAlpha);
        }
    }
    public void UnselectItem()
    {
        if (Icon)
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, nonChosenIconAlpha);
        }
    }
}
