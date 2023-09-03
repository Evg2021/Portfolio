using Cinemachine;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BinderController : MonoBehaviour
{
    public event Action OnClick;

    [SerializeField]
    private float movementSpeed = 2;

    [SerializeField]
    private float rotationSpeed = 0.5f;

    [SerializeField]
    private float speedKoeff = 3;

    private StarterAssetsInputs input;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private float TopClamp = 70.0f;
    private float BottomClamp = -30.0f;

    private bool leftClickIsDown;

    private const float threshold = 0.01f;

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        input.OnLeftClickUp += Input_OnLeftClickUp;
        input.OnLeftClickDown += Input_OnLeftClickDown;
        input.OnRightClickUp += Input_OnRightClickUp;
    }

    private void Input_OnRightClickUp()
    {
        if (!BindingManager.IsBindingPanelOpened)
        {
            enabled = !enabled;

            if (enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void Input_OnLeftClickDown()
    {
        if (enabled && !EventSystem.current.IsPointerOverGameObject())
        {
            leftClickIsDown = true;
        }
    }

    private void Input_OnLeftClickUp()
    {
        if (enabled && !EventSystem.current.IsPointerOverGameObject() && leftClickIsDown)
        {
            leftClickIsDown = false;
            OnClick?.Invoke();
        }
    }

    private void Update()
    {
        Move();
        RotateCamera();
    }

    private void Move()
    {
        if (input)
        {
            var direction = Vector3.zero;

            if (input.move != Vector2.zero)
            {
                direction.x = input.move.x;
                direction.z = input.move.y;
            }

            if (input.height != 0.0f)
            {
                direction.y = input.height;
            }

            direction = transform.TransformDirection(direction);

            float currentSpeed = input.sprint ? movementSpeed * speedKoeff : movementSpeed;
            currentSpeed = input.crouching ? currentSpeed / speedKoeff : currentSpeed;

            transform.position += direction.normalized * currentSpeed * Time.deltaTime;
        }
    }
    private void RotateCamera()
    {
        if (input)
        {
            if (input.look.sqrMagnitude >= threshold)
            {
                var currentSpeed = input.crouching ? rotationSpeed / speedKoeff : rotationSpeed;

                cinemachineTargetYaw += input.look.x * Time.deltaTime * currentSpeed;
                cinemachineTargetPitch += input.look.y * Time.deltaTime * currentSpeed;
            }

            cinemachineTargetYaw = Utilities.ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = Utilities.ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
        }
    }
}
