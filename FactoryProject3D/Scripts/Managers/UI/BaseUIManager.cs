using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUIManager : SingletonMonoBehaviour<BaseUIManager>
{
    [SerializeField]
    private NotificationController NotificationController;

    private void OnEnable()
    {
        if (!NotificationController)
        {
            NotificationController = FindObjectOfType<NotificationController>();
        }

        if (NotificationController)
        {
            NotificationController.gameObject.SetActive(false);
        }
    }

    public virtual void ShowNotification(string message)
    {
        if (NotificationController)
        {
            NotificationController.gameObject.SetActive(true);
            NotificationController.SetMessage(message);
        }
    }
}
