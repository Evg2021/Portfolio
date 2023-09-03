using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateViewController : MonoBehaviour
{
    private const string trenObjectPlaceName = "TrenObjectPlace";
    public const string templateCameraName = "TemplateCamera";

    public Transform TrenObjectPlace { get; private set; }

    public void Initialize()
    {
        InitializeTrenObjectPlace();
    }
    private void InitializeTrenObjectPlace()
    {
        TrenObjectPlace = transform.Find(trenObjectPlaceName);
    }
    public void InitializeTemplateCanvas()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {            
            canvas.worldCamera = GetComponentInChildren<Camera>();
         }
    }    
}
