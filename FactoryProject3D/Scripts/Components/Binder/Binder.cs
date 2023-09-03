using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Binder : SingletonMonoBehaviour<Binder>
{
    [SerializeField]
    private float raycastDistance = 1000.0f;

    [SerializeField]
    private string[] ControllersNames = { "Manipulator", "button", "Manometr", "handle", "swinger"};

    private LayerMask interactableLayer;
    private string interactableLayerName = "Interactable";
    private Color controllerColor = Color.yellow * 2.0f;
    private string emissionShaderName = "_EmissionColor";
    private static string emissionKeyword = "_EMISSION";
    private static string aimName = "Aim";
    private static string infoAimName = "MainInfo";

    private Ray raycast;
    private Vector3 rayPoint;
    private Camera camera;
    private BinderController binderController;
    private TextMeshProUGUI infoText;

    private Transform currentController;
    private InteractableObject currentInteractable;

    private MeshRenderer controllerRenderer;
    private Material originalControllerMaterial;

    private GameObject currentOutlineObject;
    private GameObject fixedOutlineObject;
    private bool pipeIsFixed;

    private void Start()
    {
        camera = GetComponent<Camera>();
        interactableLayer = LayerMask.NameToLayer(interactableLayerName);

        binderController = GetComponent<BinderController>();
        if (binderController)
        {
            binderController.OnClick += BinderController_OnClick;
        }

        var aim = GameObject.Find(aimName);
        if (aim)
        {
            var info = aim.transform.Find(infoAimName);
            if (info && info.TryGetComponent(out infoText))
            {
                infoText.text = string.Empty;
            }
        }
    }

    private void BinderController_OnClick()
    {
        if (!BindingManager.IsBindingPanelOpened)
        {
            if (fixedOutlineObject)
            {
                Destroy(fixedOutlineObject);
            }

            if (currentController && !currentInteractable)
            {
                if (currentController.name.Contains("pipe"))
                {
                    Destroy(currentOutlineObject);

                    if(controllerRenderer)
                    {
                        fixedOutlineObject = Utilities.GenerateOutline(controllerRenderer, 
                            GlobalVariables.InteractableObjectHighlightMaterial);
                    }
                }
                else
                {
                    BindingManager.SetCurrentController(currentController);
                }
            }
            else if (currentInteractable && !currentController)
            {
                BindingManager.SetCurrentInteractableObject(currentInteractable);
            }
        }
    }

    private void Update()
    {
        FindInteractableObject();
    }

    private void FindInteractableObject()
    {
        if (camera)
        {
            bool cursorDeattached = binderController && !binderController.enabled;
            rayPoint = cursorDeattached ? Input.mousePosition : new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
            raycast = camera.ScreenPointToRay(rayPoint);
            if (Physics.Raycast(raycast, out var hit, raycastDistance))
            {
                if (infoText)
                {
                    infoText.text = hit.transform.name;
                }

                if (hit.transform.gameObject.layer == interactableLayer)
                {
                    if (ControllersNames.Any(hit.transform.name.Contains))
                    {
                        DisableControllerHighlight();
                        DisableCurrentInteractable();

                        EnableControllerHighlight(hit.transform);
                        currentController = hit.transform;
                    }
                    else
                    {
                        DisableControllerHighlight();

                        if (hit.transform.GetComponentInParent<InteractableObject>() != currentInteractable)
                        {
                            DisableCurrentInteractable();
                            EnableInteractable(hit.transform.GetComponentInParent<InteractableObject>());
                        }
                    }
                }
                else
                {
                    DisableControllerHighlight();
                    DisableCurrentInteractable();
                }
            }
            else
            {
                DisableControllerHighlight();
                DisableCurrentInteractable();

                if (infoText)
                {
                    if (!string.IsNullOrEmpty(infoText.text))
                    {
                        infoText.text = string.Empty;
                    }
                }
            }
        }
    }

    private void EnableControllerHighlight(Transform controller)
    {
        if (controller != null)
        {
            if (controller.TryGetComponent(out controllerRenderer))
            {
                currentOutlineObject = Utilities.GenerateOutline(controllerRenderer, GlobalVariables.EnvironmentOutlineHighlightMaterial);
            }
        }
    }
    private void DisableControllerHighlight()
    {
        if (currentOutlineObject)
        {
            Destroy(currentOutlineObject);
        }

        if (controllerRenderer)
        {
            controllerRenderer = null;
        }

        if (currentController)
        {
            currentController = null;
        }
    }
    private void EnableInteractable(InteractableObject interactable)
    {
        if (interactable)
        {
            currentInteractable = interactable;
            currentInteractable.EnableHighlight();
        }
    }
    private void DisableCurrentInteractable()
    {
        if (currentInteractable)
        {
            currentInteractable.DisableHighlight();
            currentInteractable = null;
        }
    }

    public void EnableController()
    {
        SetActiveController(true);
    }
    public void DisableController()
    {
        SetActiveController(false);
    }
    private void SetActiveController(bool value)
    {
        if (binderController)
        {
            binderController.enabled = value;
        }
    }
}
