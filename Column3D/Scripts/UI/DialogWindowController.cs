using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindowController : SingletonMonoBehaviour<DialogWindowController>
{
    public bool IsInitialized { get; private set; }

    private Transform content;
    private Text dialogItemPrefab;
    private GameObject[] dialogItems;
    int count = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(string[] messages)
    {
        if (content == null)
        {
            content = GetComponentInChildren<ScrollRect>().content.transform;
        }
        else if (content != null)
        {
            RemoveAllMessages();
        }
        if (!Utility.GetDialogWindowItemPrefab().TryGetComponent<Text>(out dialogItemPrefab) || content == null)
        {
            string errorMessage = string.Empty;
            if (content == null)
            {
                errorMessage += " Content is missing.";
            }

            if (dialogItemPrefab == null)
            {
                errorMessage += " DialogItem Prefab has no LayoutElement Component.";
            }

            Debug.LogError("Dialog Window Error Initialization:" + errorMessage);
        }
        else
        {
            dialogItems = new GameObject[messages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                AddMessage(i, messages[i]);
            }
        }
    }

    public void AddMessage(int key, string message)
    {
        if (content != null && dialogItemPrefab != null)
        {
            GameObject item = null;
            if (!string.IsNullOrEmpty(message))
            {
                item = Instantiate(dialogItemPrefab.gameObject, content);
                item.GetComponent<Text>().text = message;
                item.transform.SetSiblingIndex(0);
            }

            dialogItems[key] = item;
        }
    }
    public void RemoveAllMessages()
    {
        if (dialogItems != null)
        {
            foreach (var dialogItem in dialogItems)
            {
                if (dialogItem != null)
                {
                    Destroy(dialogItem);

                }
            }
            dialogItems = null;
        }
    }
    public void ClearWindow()
    {
        if (dialogItems != null)
        {
            foreach (var dialogItem in dialogItems)
            {
                if (dialogItem != null)
                {
                    dialogItem.SetActive(false);
                }
            }
        }
    }
    public void ShowMessage(int key)
    {
        if (dialogItems != null)
        {
            if (dialogItems[key] != null)
            {
                dialogItems[key].SetActive(true);
            }
        }
    }
    public void HideMesasge(int key)
    {
        if (dialogItems != null)
        {
            if (dialogItems[key] != null)
            {
                dialogItems[key].SetActive(false);
            }
        }
    }
    public void ShowAllMessages()
    {
        if (dialogItems != null)
        {
            for (int i = 0; i < dialogItems.Length; i++)
            {
                ShowMessage(i);
            }
        }
    }
    public void ShowAllMessagesBefore(int key)
    {
        if (dialogItems != null)
        {
            int count = key;
            if (count > dialogItems.Length)
            {
                count = dialogItems.Length;
            }

            for (int i = 0; i <= count; i++)
            {
                ShowMessage(i);
            }
        }
    }
}
