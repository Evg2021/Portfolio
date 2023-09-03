using StarterAssets;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : NetworkBehaviour
{
    public float Offset = 0.13f;

    private const string cameraRootName = "PlayerCameraRoot";
    private const string headWord = "head";

    private Transform playerCameraRoot;
    private Transform playerHead;
    private float cameraRootPosition;

    private Animator animator;
    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private CameraShake cameraShake;

    private void Start()
    {
        if (IsOwner)
        {
            InitializeComponents();
        }
    }

    private void Update()
    {
        if (playerCameraRoot && input && controller && animator)
        {
            if (playerHead)
            {
                if ((input.move != Vector2.zero) && controller.Grounded && !controller.CamLowerPosFlag && controller.enabled && !controller.CheckCurrentAnimatorClip("crouch"))
                {
                    // Избавиться от offset
                    playerCameraRoot.transform.position = new Vector3(playerHead.transform.position.x,
                                                              playerHead.transform.position.y + Offset,
                                                              playerCameraRoot.transform.position.z);
                }
                else if (playerCameraRoot.transform.position.y != cameraRootPosition && !controller.CamLowerPosFlag)
                {
                    playerCameraRoot.transform.position = new Vector3(playerCameraRoot.transform.position.x,
                                                              cameraRootPosition + transform.position.y + Offset,
                                                              playerCameraRoot.transform.position.z); ;
                }
            }
            else
            {
                Debug.LogError("PlayerHead is missing");
            }

        }
    }

    private void InitializeComponents()
    {
        // поиск среди детей игрока по имени 
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (child.name.ToLower().Contains(headWord.ToLower()))
            {
                playerHead = child;
            }
        }

        playerCameraRoot = transform.Find(cameraRootName);

        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        cameraShake = GetComponent<CameraShake>();

        if (playerCameraRoot && playerHead)
        {
            playerCameraRoot.transform.position = new Vector3(playerHead.transform.position.x, 
                                                              playerHead.transform.position.y, 
                                                              playerHead.transform.position.z);

            cameraRootPosition = playerCameraRoot.transform.position.y;
        }

        controller.OnControllerEnable += () => { cameraShake.enabled = true; };
        controller.OnControllerDisable += () => { cameraShake.enabled = false; };
    }
}
