using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateCanvasController : MonoBehaviour
{
    private Canvas templateCanvas;

    private void Awake()
    {
        templateCanvas = GetComponent<Canvas>();
        if (templateCanvas)
        {
            templateCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            templateCanvas.worldCamera = GameObject.Find(TemplatePanelController.templateCameraName)?.GetComponent<Camera>();
        }
    }
}
