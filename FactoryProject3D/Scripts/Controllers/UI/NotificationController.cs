using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    private TextMeshProUGUI MessageComponent;

    public void SetMessage(string message)
    {
        if (!MessageComponent)
        {
            MessageComponent = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (MessageComponent)
        {
            MessageComponent.text = message;
        }
        else
        {
            Debug.LogError("MessageComponent TextMeshPro is missing on " + transform.name + " object.");
        }
    }
}
