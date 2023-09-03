using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AVOFanController : MonoBehaviour
{
    public float RotationSpeed = 500.0f;
    public float LaunchSpeed = 0.5f;
    public AnimationCurve LaunchingCurve;
    public Vector3 RotationVector = Vector3.up;

    private ControllerBase controller;
    private float lerpValue = 0.0f;

    private void Start()
    {
        var interactable = GetComponentInParent<InteractableObject>();
        if (interactable)
        {
            controller = interactable.secondController;
        }
    }

    private void Update()
    {
        if (controller && controller.isEnabled && controller.GetControllerType() == Types.TYPE_BOOL)
        {
            bool value = controller.GetSimulatorValue();
            if (value)
            {
                if (lerpValue < 1.0f)
                {
                    lerpValue += Time.deltaTime * LaunchSpeed;
                }
            }
            else
            {
                if (lerpValue > 0.0f)
                {
                    lerpValue -= Time.deltaTime * LaunchSpeed;
                }
            }

            if (lerpValue > 0.0f)
            {
                transform.Rotate(RotationVector, Mathf.Lerp(0.0f, RotationSpeed, LaunchingCurve.Evaluate(lerpValue)) * Time.deltaTime);
            }
        }
    }
}
