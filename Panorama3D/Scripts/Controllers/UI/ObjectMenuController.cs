using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectMenuController : MonoBehaviour
{    
    [SerializeField]
    private TMP_InputField TrenNameInput;

    [SerializeField]
    private TMP_Dropdown TemplateType;

    private TemplateTypes currentTypes;          

    private void Awake()
    {
        currentTypes = FindTemplatesTypes();     
        GetNameTemplatesTypesList();
    }
    public void ClearFields()
    {
        if (TrenNameInput)
        {
            TrenNameInput.text = string.Empty;
        }

        if (TemplateType)
        {
            TemplateType.value = 0;
        }
    }

    private TemplateTypes FindTemplatesTypes()
    {
        return Resources.Load<TemplateTypes>(TemplatePanelController.templateTypesName);  
    }
    
    private void GetNameTemplatesTypesList()
    {
        if (currentTypes != null && currentTypes.Templates != null)
        {
            TemplateType.options.Clear();

            foreach (var element in currentTypes.Templates)
            {
                TemplateType.options.Add(new TMP_Dropdown.OptionData() {text = element.Name});
            }
        }     
    }
    public void SetCurrentTrenObject(ITrenObject trenObject)
    {
        var currentTrenName = trenObject.GetTrenName();
        var currentPultName = trenObject.GetTemplateName();

        if (TrenNameInput)
        {
            if (currentTrenName != null)
            {
                TrenNameInput.text = currentTrenName;
            }
            else
            {
                TrenNameInput.text = string.Empty;
            }
        }

        if (TemplateType)
        {
            if (!string.IsNullOrEmpty(currentPultName))
            {
                var pultItem = TemplateType.options.FirstOrDefault(h => h.text == currentPultName);
                if (pultItem != null)
                {
                    TemplateType.value = TemplateType.options.IndexOf(pultItem);
                }
            }
            else
            {
                TemplateType.value = 0;
            }
        }
    }

    public void OnClickApproveObjectButton()
    {
        var currentInteractableObject = UIController.Instance?.CurrentInteractableObject;
        if (currentInteractableObject != null)
        {
            var currentTrenObject = currentInteractableObject as ITrenObject;
            if (currentTrenObject != null)
            {
                if (!string.IsNullOrEmpty(TrenNameInput?.text) && TemplateType)
                {
                    string name = TrenNameInput.text;
                    string pultType = TemplateType.options[TemplateType.value].text;

                    currentTrenObject.SetTrenObjectName(name);
                    currentTrenObject.SetPultObjectName(pultType);

                    PanoramaController.Instance.ApproveTrenObject(currentTrenObject);
                    CameraController.Instance.EnableControl();
                }
            }
        }
    }
}
