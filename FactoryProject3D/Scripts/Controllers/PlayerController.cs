using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    private const float transformThreshold = 0.01f;
    private const float transformSyncSpeed = 10.0f;

    [SerializeField]
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();

    [SerializeField]
    private NetworkVariable<Vector2> networkMove = new NetworkVariable<Vector2>();

    [SerializeField]
    private NetworkVariable<Vector2> networkLook = new NetworkVariable<Vector2>();

    [SerializeField]
    private NetworkVariable<bool> networkJump = new NetworkVariable<bool>();

    [SerializeField]
    private NetworkVariable<bool> networkSprint = new NetworkVariable<bool>();

    [SerializeField]
    private NetworkVariable<bool> networkCrouching = new NetworkVariable<bool>();

    [SerializeField]
    private NetworkVariable<float> networkDirectionY = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<Quaternion> networkCurrentManipulator = new NetworkVariable<Quaternion>();

    public float Speed;
    public Transform CameraRoot;

    private Vector2 oldMove;
    private Vector2 oldLook;

    public StarterAssetsInputs input { get; private set; }
    public ThirdPersonController controller { get; private set; }
    private Interactor interactor;

    private static string ownPlayerLayerName = "OwnPlayer";

    public bool OnPause { get; private set; }

    private bool isVR = false;

    private void Start()
    {
        InitializeInput();
        InitializeCameraFollower();
        InitializeThirdPersonalController();
        InitializePlayerInput();
        InitializeLayer();
        InitializeInteractor();
        InitializePauseMenu();

        Cursor.lockState = CursorLockMode.Locked;
        OnPause = false;

        InitializeVR();
    }
    private void InitializePauseMenu()
    {
        if (PauseMenuController.Instance && IsOwner)
        {
            PauseMenuController.Instance.ownerController = this;
        }
    }
    private void InitializeInteractor()
    {
        interactor = GetComponent<Interactor>();
        if (!interactor)
        {
            Debug.LogError($"Player has no {nameof(interactor)} component");
        }
        else
        {
            interactor.OnInteractTrenObject += Interactor_OnInteract;
        }
    }
    private void InitializeLayer()
    {
        if (IsOwner)
        {
            var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers != null && renderers.Length > 0)
            {
                LayerMask layer = LayerMask.NameToLayer(ownPlayerLayerName);
                foreach (var renderer in renderers)
                {
                    renderer.gameObject.layer = layer;
                }
            }
        }
    }
    private void InitializeThirdPersonalController()
    {
        controller = GetComponent<ThirdPersonController>();

        if (!controller)
        {
            Debug.LogError($"{transform.name} has no {nameof(ThirdPersonController)} component.");
        }
    }
    private void InitializeCameraFollower()
    {
        if (IsClient && IsOwner)
        {
            if (CameraFollowerController.IsInitialized)
            {
                if (CameraRoot)
                {
                    CameraFollowerController.SetFollowObject(CameraRoot);
                }
                else
                {
                    Debug.LogError($"{transform.name} has no transform for {nameof(CameraFollowerController)} component.");
                }
            }
        }
    }
    private void InitializeInput()
    {
        input = GetComponent<StarterAssetsInputs>();

        if (!input)
        {
            Debug.LogError($"{nameof(StarterAssetsInputs)} component is missing on {transform.name} object.");
        }
        else
        {
            input.OnPausePressed += Input_OnPausePressed;
        }
    }
    private void InitializePlayerInput()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }
    private void InitializeVR()
    {
        if (IsOwner)
        {
            var vr = GameObject.Find("SteamVR Player");
            if (vr)
            {
                vr.transform.parent = null;
                if (vr.TryGetComponent<ObjectFollower>(out var follower))
                {
                    follower.InitializeTarget(transform);
                }
                interactor.enabled = false;

                controller._mainCamera = vr.GetComponentInChildren<Camera>()?.gameObject;

                isVR = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            CheckInput();
            SendTransform();
        }
    }

    private void SendTransform()
    {
        if ((Mathf.Abs(transform.position.sqrMagnitude - networkPosition.Value.sqrMagnitude) > transformThreshold) ||
            (Mathf.Abs(Quaternion.Dot(transform.rotation, networkRotation.Value))            > transformThreshold)  )
        {
            UpdateTransformServerRpc(transform.position, transform.rotation);
        }
    }

    [ServerRpc]
    private void UpdateTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;

        if (Mathf.Abs(transform.position.sqrMagnitude - networkPosition.Value.sqrMagnitude) > transformThreshold ||
            Mathf.Abs(Quaternion.Dot(transform.rotation, networkRotation.Value))            > transformThreshold  )
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * transformSyncSpeed);
            UpdateTransformClientRpc();
        }
    }

    [ClientRpc]
    private void UpdateTransformClientRpc()
    {
        if (!IsOwner)
        {
            if (Mathf.Abs(transform.position.sqrMagnitude - networkPosition.Value.sqrMagnitude) > transformThreshold ||
                Mathf.Abs(Quaternion.Dot(transform.rotation, networkRotation.Value))            > transformThreshold  )
            {
                transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * transformSyncSpeed);
            }
        }
    }

    private void CheckInput()
    {
        if (input)
        {
            if (networkMove.Value != input.move ||
                networkLook.Value != input.look ||
                Camera.main.transform.eulerAngles.y != networkDirectionY.Value ||
                networkJump.Value != input.jump ||
                networkSprint.Value != input.sprint ||
                networkCrouching.Value != input.crouching)
            {
                UpdateInputServerRpc(input.move, input.look, Camera.main.transform.eulerAngles.y, input.jump, input.sprint, input.crouching);
            }
        }
    }

    [ServerRpc]
    private void UpdateInputServerRpc(Vector2 move, Vector2 look, float directionY, bool jump, bool sprint, bool crouching)
    {
        networkMove.Value       = move;
        networkLook.Value       = look;
        networkDirectionY.Value = directionY;
        networkJump.Value = jump;
        networkSprint.Value = sprint;
        networkCrouching.Value = crouching;

        if (input)
        {
            input.move = move;
            input.look = look;
            input.jump = jump;
            input.sprint = sprint;
            input.crouching = crouching;
        }

        if (controller)
        {
            controller.DirectionY = directionY;
        }

        UpdateInputClientRpc(move, look, directionY);
    }

    [ClientRpc]
    private void UpdateInputClientRpc(Vector2 move, Vector2 look, float directionY)
    {
        if (IsClient && !IsOwner)
        {
            if (input)
            {
                input.move = networkMove.Value;
                input.look = networkLook.Value;
                input.jump = networkJump.Value;
                input.sprint = networkSprint.Value;
                input.crouching = networkCrouching.Value;
            }

            if (controller)
            {
                controller.DirectionY = networkDirectionY.Value;
            }
        }
    }

    [ServerRpc]
    private void UpdateTrenControllerServerRpc(uint index, float value)
    {
        UpdateTrenControllerClientRpc(index, value);
    }

    [ClientRpc]
    private void UpdateTrenControllerClientRpc(uint index, float value)
    {
        if (!IsOwner)
        {
            if (TrenObjectInitializator.Instance)
            {
                var trenObjects = TrenObjectInitializator.Instance.TrenObjects;
                if (trenObjects != null && trenObjects.Length > index)
                {
                    var currentTrenObject = trenObjects[index];
                    if (currentTrenObject && currentTrenObject.TryGetComponent<IController>(out var controller))
                    {
                        controller.Interact(value, false);
                    }
                }
            }
        }
    }

    private void Interactor_OnInteract(uint index, float value)
    {
        /*if (IsClient && IsOwner)
        {
            UpdateTrenControllerServerRpc(index, value);
        }*/
    }

    private void Input_OnPausePressed()
    {
        if (controller && !controller.enabled && !OnPause)
        {
            if (!interactor.DeattachedCursor)
            {
                EnableTPController();
            }

            if (interactor)
            {
                interactor.HidePultOfCurrentInteractable();
            }
        }
        else
        {
            SetPause();
        }
    }

    public void SetPause()
    {
        if (IsOwner)
        {
            OnPause = !OnPause;

            SetActiveTPController(!OnPause);

            if (PauseMenuController.Instance)
            {
                if (OnPause)
                {
                    PauseMenuController.Instance.ShowPauseMenu();
                }
                else
                {
                    PauseMenuController.Instance.HidePauseMenu();
                }
            }

            Cursor.lockState = OnPause ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void EnableTPController()
    {
        SetActiveTPController(true);
        SetActiveTPControllerServerRpc(true);
    }
    public void DisableTPController()
    {
        SetActiveTPController(false);
        SetActiveTPControllerServerRpc(false);
    }
    private void SetActiveTPController(bool value)
    {
        if (controller && controller.enabled != value)
        {
            controller.enabled = value;
        }
    }

    [ServerRpc]
    private void SetActiveTPControllerServerRpc(bool value)
    {
        SetActiveTPControllerClientRpc(value);
    }
    [ClientRpc]
    private void SetActiveTPControllerClientRpc(bool value)
    {
        if (!IsOwner)
        {
            SetActiveTPController(value);
        }
    }

    public void OnFlyingToggleSet(bool value)
    {
        Debug.Log(value);
    }

    private void OnGUI()
    {
        if (IsOwner)
        {
            GUI.Box(new Rect(Screen.width - 150, 10, 140, 25), $"X:{Mathf.Round(transform.position.x * 10) / 10}, " +
                                                               $"Y:{Mathf.Round(transform.position.y * 10) / 10}, " +
                                                               $"Z:{Mathf.Round(transform.position.z * 10) / 10}");
        }
    }
}
