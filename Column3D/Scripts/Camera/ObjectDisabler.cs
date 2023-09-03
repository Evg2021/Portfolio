using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    public float TriggerDistance = 3.9f;
    public float debug;

    private bool isEnabled;
    private MeshRenderer[] renderers;
    private Collider[] colliders;

    private void Awake()
    {
        isEnabled = false;
        renderers = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Enable()
    {
        isEnabled = true;
    }
    public void Disable()
    {
        isEnabled = false;

        Show();
    }

    public void Show()
    {
        if (renderers != null)
        {
            foreach (var render in renderers)
            {
                if (render != null && !render.enabled)
                {
                    render.enabled = true;
                }
            }
        }

        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }
    public void Hide()
    {
        if (renderers != null)
        {
            foreach (var render in renderers)
            {
                if (render != null && render.enabled)
                {
                    render.enabled = false;
                }
            }
        }

        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (isEnabled)
        {
            var camera = Camera.main;
            if (camera != null)
            {
                var ownPositionXZ = new Vector2(transform.position.x, transform.position.z);
                var cameraPositionXZ = new Vector2(camera.transform.position.x, camera.transform.position.z);
                var distance = Vector2.Distance(ownPositionXZ, cameraPositionXZ);
                if (distance <= TriggerDistance)
                {
                    Hide();
                }
                else if (distance > TriggerDistance)
                {
                    Show();
                }

                debug = distance;
            }
        }
    }
}
