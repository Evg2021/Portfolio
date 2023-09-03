using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterController : MonoBehaviour
{
    public float TriggerDistance = 5.0f;

    private ObiEmitter emitter;
    private bool isEnableByCameraDistance;
    private new Transform camera;

    private void Awake()
    {
        emitter = GetComponentInChildren<ObiEmitter>();
        isEnableByCameraDistance = false;
    }

    private void Start()
    {
        StopEmitter();
    }

    public void StopEmitter()
    {
        if (emitter != null && emitter.gameObject.activeSelf)
        {
            emitter.gameObject.SetActive(false);
        }
    }

    public void StartEmitter()
    {
        if (emitter != null && !emitter.gameObject.activeSelf)
        {
            emitter.gameObject.SetActive(true);
        }
    }

    public void EnableByCameraDistance()
    {
        camera = Camera.main.transform;
        if (camera != null)
        {
            isEnableByCameraDistance = true;
        }
    }

    public void DisableEnablingByCameraDistance()
    {
        StopEmitter();
        isEnableByCameraDistance = false;
    }

    private void Update()
    {
        if (isEnableByCameraDistance && emitter != null)
        {
            var distance = Vector3.Distance(camera.position, emitter.transform.position);
            if (distance <= TriggerDistance)
            {
                StartEmitter();
            }
            else if (distance > TriggerDistance)
            {
                StopEmitter();
            }
        }
    }
}
