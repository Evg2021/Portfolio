using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private GameObject descriptionWindowPrefab;
    private static List<GameObject> descriptionWindows;
    private static int maxWindowsCount = 10;
    private static Vector2 offset;

    private void Awake()
    {
        descriptionWindowPrefab = Utility.GetDescriptionWindow2DPrefab();
        descriptionWindows = new List<GameObject>();
        if (descriptionWindowPrefab != null)
        {
            if (descriptionWindowPrefab.TryGetComponent<RectTransform>(out var rect))
            {
                offset = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y) * -0.125f;
            }
        }
    }

    public GameObject InstantiateDescriptionWindow()
    {
        if (descriptionWindows == null)
        {
            descriptionWindows = new List<GameObject>();
        }

        if (descriptionWindowPrefab != null)
        {
            var window = Instantiate(descriptionWindowPrefab, transform);
            var offsetVector3 = new Vector3(offset.x, offset.y, 0.0f);
            window.transform.position += offsetVector3 * descriptionWindows.Count;
            descriptionWindows.Add(window);

            if (descriptionWindows.Count >= maxWindowsCount)
            {
                if (descriptionWindows[0] != null)
                {
                    if (descriptionWindows[0].TryGetComponent<DescriptionController>(out var controller))
                    {
                        controller.Destroy();
                    }
                }
                descriptionWindows.RemoveAt(0);
            }

            return window;
        }
        else
        {
            Debug.LogError("DescriptionWindow2D prefab is missing.");
            return null;
        }
    }

    public static void RemoveDescriptionWindow(GameObject window)
    {
        if (descriptionWindows != null)
        {
            if (descriptionWindows.Contains(window))
            {
                descriptionWindows.Remove(window);
            }
        }
    }

    public void RemoveDescriptionWindows()
    {

    }
}
