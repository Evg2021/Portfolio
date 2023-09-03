using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BindingUIController : MonoBehaviour
{
    private const string showBindingsHeader = "Показать привязанное";
    private const string hideBindingsHeader = "Спрятать привязанное";

    private const string showCheckedBindingsHeader = "Проверить";
    private const string hideCheckedBindingsHeader = "Выключить проверку";

    [SerializeField]
    private TextMeshProUGUI showBindingsButtonHeader;

    [SerializeField]
    private TextMeshProUGUI showCheckedBindingsButtonHeader;

    [SerializeField]
    private TextMeshProUGUI bindingsCount;

    [SerializeField]
    private TextMeshProUGUI objectsCount;

    public void OnClickCloseButton()
    {
        BindingManager.SaveCurrentBindings();
        Application.Quit();
    }

    public void OnClickShowBindings()
    {
        if (!BindingManager.IsBindingsShown)
        {
            BindingManager.ShowBindedObjects();

            if (showBindingsButtonHeader)
            {
                showBindingsButtonHeader.text = hideBindingsHeader;
            }
        }
        else
        {
            if (BindingManager.isConnectedToSim)
            {
                OnClickShowCheckedBindings();
            }

            BindingManager.HideBindedObjects();

            if (showBindingsButtonHeader)
            {
                showBindingsButtonHeader.text = showBindingsHeader;
            }
        }
    }

    public void SetBindingsCount(int count)
    {
        if (bindingsCount)
        {
            bindingsCount.text = count.ToString();
        }
    }

    public void SetObjectsCount(int count)
    {
        if (objectsCount)
        {
            objectsCount.text = count.ToString();
        }
    }

    public void OnClickShowCheckedBindings()
    {
        if (!BindingManager.isConnectedToSim)
        {
            if(!BindingManager.IsBindingsShown)
            {
                OnClickShowBindings();
            }

            BindingManager.ConnectToSim();

            if (showCheckedBindingsButtonHeader)
            {
                showCheckedBindingsButtonHeader.text = hideCheckedBindingsHeader;
            }
        }
        else
        {
            BindingManager.DisconnectFromSim();

            if (showCheckedBindingsButtonHeader)
            {
                showCheckedBindingsButtonHeader.text = showCheckedBindingsHeader;
            }
        }
    }
}
