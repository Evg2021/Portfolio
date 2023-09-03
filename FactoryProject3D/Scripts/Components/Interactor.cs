using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactor : NetworkBehaviour
{
    private const string interactableSecondInfoPrefix = "Нажмите E чтобы ";
    private const string interactableWithPultDescription = "Нажмите ЛКМ чтобы открыть пульт";
    private const string interactableLayerName = "Interactable";
    private const string interactionMaskName = "Overpass";

    private const float widthDeattachedCursorLabel = 135.0f;
    private const float heightDeattachedCursorLabel = 25.0f;

    public event Action<uint, float> OnInteractTrenObject;
    public event Action<EnvironmentControllerBase> OnInteractEnvironment;

    public float detectionDistance = 5.0f;
    public TextMeshProUGUI mainInfo;
    public TextMeshProUGUI secondInfo;

    private IInteractable currentInteractable;
    private IInteractable previousInteractableOnFirstInteraction;
    private IInteractable previousInteractableOnSecondInteraction;

    private Ray raycast;
    private Vector3 rayPoint;

    private StarterAssetsInputs input;
    private PlayerController playerController;

    private static LayerMask interactableLayer;
    public LayerMask detectableLayers;

    public bool DeattachedCursor { get; private set; }

    private void OnValidate()
    {
        detectableLayers = ~((1 << gameObject.layer) + (1 <<LayerMask.NameToLayer(interactionMaskName)));
    }

    private void Start()
    {
        if (IsOwner)
        {
            interactableLayer = LayerMask.NameToLayer(interactableLayerName);

            InitializeInput();
            InitializePlayerController();
            InitializeInfoTextComponent();

            DeattachedCursor = false;
        }
        else
        {
            enabled = false;
        }
    }

    private void InitializePlayerController()
    {
        playerController = GetComponent<PlayerController>();
        if (!playerController)
        {
            Debug.Log($"{nameof(playerController)} was no found on {transform.name}");
        }
    }
    private void InitializeInput()
    {
        input = GetComponent<StarterAssetsInputs>();

        if (!input)
        {
            Debug.LogError($"{transform.name} has no {nameof(input)} component.");
        }
        else
        {
            input.OnSecondInteractionDown += Input_OnSecondInteractionDown;
            input.OnSecondInteractionUp += Input_OnSecondInteractionUp;

            input.OnFirstInteractionDown += Input_OnFirstInteractionDown;
            input.OnFirstInteractionUp += Input_OnFirstInteractionUp;

            input.OnRightClickDown += SetCursorAttachmentState;
        }
    }
    private void InitializeInfoTextComponent()
    {
        var aimObject = GameObject.Find(GlobalVariables.AimObjectName);
        if (aimObject)
        {
            var mainInfoObject = aimObject.transform.Find(GlobalVariables.MainInfoObjectName);
            if (!mainInfoObject || !mainInfoObject.TryGetComponent(out mainInfo))
            {
                Debug.LogError($"Error in searching {GlobalVariables.MainInfoObjectName} object in {aimObject.name}.");
            }

            var secondInfoObject = aimObject.transform.Find(GlobalVariables.SecondInfoObjectName);
            if (!secondInfoObject || !secondInfoObject.TryGetComponent(out secondInfo))
            {
                Debug.LogError($"Error in searching {GlobalVariables.SecondInfoObjectName} object in {aimObject.name}.");
            }
        }
    }
    public void InitializeDetectableLayers()
    {
        detectableLayers = ~((1 << gameObject.layer) + (1 << LayerMask.NameToLayer(interactionMaskName)));
    }

    void Update()
    {
        if (IsOwner)
        {
            FindInteractableObject();
            ShowMainInfo();
            ShowSecondInfo();
            ReadInput();
            UpdateControllers();
        }
    }

    private void FindInteractableObject()
    {
        rayPoint = DeattachedCursor ? Input.mousePosition : new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
        raycast = Camera.main.ScreenPointToRay(rayPoint);
        if (Physics.Raycast(raycast, out var hit, detectionDistance, detectableLayers))
        {
            if (hit.transform.gameObject.layer == interactableLayer)
            {
                if (!hit.transform.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable = hit.transform.GetComponentInParent<IInteractable>();
                }

                if (currentInteractable != null && currentInteractable != interactable)
                {
                    DisableHighlightCurrentInteractable();
                    currentInteractable = null;
                }

                if (interactable != null)
                {
                    currentInteractable = interactable;
                    EnableHighlightCurrentInteractable();
                }
            }
            else
            {
                if (currentInteractable != null)
                {
                    DisableHighlightCurrentInteractable();
                    currentInteractable = null;
                }
            }

        }
        else
        {
            if (currentInteractable != null)
            {
                DisableHighlightCurrentInteractable();
                currentInteractable = null;
            }
        }
    }
    private void ShowMainInfo()
    {
        if (mainInfo)
        {
            if (currentInteractable != null)
            {
                if (currentInteractable is InteractableObject)
                {
                    var controller = currentInteractable.GetMainController();
                    if (controller != null)
                    {
                        var value = controller.GetSimulatorValue();
                        var name = controller.GetTrenName();
                        if (value != null || !string.IsNullOrEmpty(name))
                        {
                            if (controller.GetTypesCount() == 1)
                            {
                                mainInfo.text = $"{name} \n {value}";

                                if (controller.GetControllerType() == Types.TYPE_FLOAT && controller.GetType() != typeof(ValuesShowingController))
                                {
                                    mainInfo.text += '%';
                                }
                            }
                            else if (controller.GetTypesCount() == 2)
                            {
                                var multiTypesController = controller as IMultiTypesController;
                                if (multiTypesController != null)
                                {
                                    if (multiTypesController.GetControllerTypeGet() == Types.TYPE_BOOL)
                                    {
                                        var customInfoController = controller as ICustomInfoController;
                                        if (customInfoController != null)
                                        {
                                            mainInfo.text = $"{name} \n {customInfoController.GetPostfix()}";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var secondController = currentInteractable.GetSecondController();
                        if (secondController == null)
                        {
                            mainInfo.text = string.Empty;
                        }
                        else
                        {
                            mainInfo.text = secondController.GetTrenName();
                        }
                    }
                } 
                else if (currentInteractable is EnvironmentControllerBase)
                {
                    var customInfoInteractable = currentInteractable as ICustomInfoController;
                    mainInfo.text = interactableSecondInfoPrefix + customInfoInteractable.GetPostfix();
                }
                else
                {
                    mainInfo.text = string.Empty;
                }
            }
            else
            {
                mainInfo.text = string.Empty;
            }
        }
    }
    private void ShowSecondInfo()
    {
        if (secondInfo)
        {
            if (currentInteractable != null)
            {
                var controller = currentInteractable.GetSecondController() as ICustomInfoController;
                if (controller != null)
                {
                    string message = interactableSecondInfoPrefix + controller.GetPostfix();
                    if (currentInteractable is InteractableObjectWithPult)
                    {
                        message += '\n' + interactableWithPultDescription;
                    }

                    secondInfo.text = message;
                }
                else
                {
                    secondInfo.text = string.Empty;
                }
            }
            else
            {
                secondInfo.text = string.Empty;
            }
        }
    }
    private void ReadInput()
    {
        if (currentInteractable != null)
        {
            if (input)
            {
                var mainController = currentInteractable.GetMainController();                

                if (mainController != null && input.scroll != 0)
                {
                    mainController.Interact(input.scroll);
                    uint index = mainController.GetTrenObjectIndex();
                    OnInteractTrenObject?.Invoke(index, input.scroll);
                }
            }
        }
    }
    private void UpdateControllers()
    {
        if (currentInteractable != null)
        {
            var mainController = currentInteractable.GetMainController();
            var secondController = currentInteractable.GetSecondController();

            if (mainController != null && mainController is ITrenObjectManipulator)
            {
                var mainManipulator = mainController as ITrenObjectManipulator;
                mainManipulator.UpdateTrenObject();
            }

            if (secondController != null && secondController is ITrenObjectManipulator)
            {
                var secondManipulator = secondController as ITrenObjectManipulator;
                secondManipulator.UpdateTrenObject();
            }
        }
    }

    private void Input_OnSecondInteractionDown()
    {
        previousInteractableOnSecondInteraction = currentInteractable;
    }
    private void Input_OnSecondInteractionUp()
    {
        if (currentInteractable != null && previousInteractableOnSecondInteraction == currentInteractable)
        {
            var secondController = currentInteractable.GetSecondController();
            if (secondController != null)
            {
                secondController.Interact(null);
            }
            else if(currentInteractable is EnvironmentControllerBase)
            {
                var environmentController = currentInteractable as EnvironmentControllerBase;
                environmentController.Interact();
                OnInteractEnvironment?.Invoke(environmentController);
            }
        }
    }

    private void Input_OnFirstInteractionDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            previousInteractableOnFirstInteraction = currentInteractable;
        }
    }
    private void Input_OnFirstInteractionUp()
    {
        if (playerController && !playerController.OnPause)
        {
            if (currentInteractable != null && previousInteractableOnFirstInteraction == currentInteractable)
            {
                var interactableWithPult = currentInteractable as InteractableObjectWithPult;
                if (interactableWithPult && interactableWithPult.PultPrefab && !interactableWithPult.IsPultShown && interactableWithPult.IsInitialized)
                {
                    interactableWithPult.ShowPult();

                    playerController.DisableTPController();

                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }

    public void HidePultOfCurrentInteractable()
    {
        if (previousInteractableOnFirstInteraction != null)
        {
            var interactableWithPult = previousInteractableOnFirstInteraction as InteractableObjectWithPult;
            if (interactableWithPult && interactableWithPult.IsPultShown)
            {
                interactableWithPult.HidePult();
            }

            if (!DeattachedCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SetCursorAttachmentState()
    {
        bool allowDeattach = true;
        if (previousInteractableOnFirstInteraction != null)
        {
            var interactableWithPult = previousInteractableOnFirstInteraction as InteractableObjectWithPult;
            allowDeattach = !(interactableWithPult && interactableWithPult.IsPultShown);
        }

        if (allowDeattach)
        {
            DeattachedCursor = !DeattachedCursor;

            Cursor.lockState = DeattachedCursor ? CursorLockMode.None : CursorLockMode.Locked;

            if (DeattachedCursor)
            {
                if (playerController)
                {
                    playerController.DisableTPController();
                }
            }
            else
            {
                if (playerController)
                {
                    playerController.EnableTPController();
                }
            }
        }
    }

    private void EnableHighlightCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.EnableHighlight();
        }
    }
    private void DisableHighlightCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.DisableHighlight();
        }
    }

    private void OnGUI()
    {
        if (DeattachedCursor)
        {
            var rect = new Rect((Screen.width - widthDeattachedCursorLabel) * 0.5f, heightDeattachedCursorLabel, widthDeattachedCursorLabel, heightDeattachedCursorLabel);
            var style = new GUIStyle("box");
            style.fontStyle = FontStyle.Bold;
            GUI.Label(rect, "Курсор отцеплен", style);
        }
    }
}
