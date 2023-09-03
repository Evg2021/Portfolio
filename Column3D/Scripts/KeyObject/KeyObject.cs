using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyObject : MonoBehaviour
{
    public Vector3 ViewPosition
    {
        get
        {
            if (ViewTransform != null)
                return ViewTransform.position;
            return Vector3.zero;
        }
    }
    public Quaternion ViewRotation
    {
        get
        {
            if (ViewTransform != null)
                return ViewTransform.rotation;
            return Quaternion.identity;
        }
    }

    public Transform ViewTransform;

    [Space(10)]
    public string Title;
    [TextArea]
    public string Description;

    protected Material SelectedMaterial;
    protected Material IndicatorMaterial;

    protected Dictionary<MeshRenderer, Material> renderers;
    protected GameObject ownWindowDescription;
    private ParticleSystem particles;
    public bool isSelected { get; protected set; }
    public bool isIndicated { get; protected set; }
    public bool isClickable;
    private Coroutine currentCoroutine;
    protected bool isMouseDownWithoutMoving;

    private static string viewPositionName = "ViewPoint";
    private static float rotationSpeed = 500.0f;
    private static float rotationValue = 720.0f;
    protected static CanvasController canvas2D;
    protected static Vector3 mousePositionOnDown = Vector3.zero;

    public Track Track
    {
        get
        {
            return new Track()
            {
                ViewPosition = this.ViewPosition,
                ViewRotation = this.ViewRotation
            };
        }
    }
    public KeyObjectDescriptionWindow DescriptionInfo
    {
        get
        {
            return new KeyObjectDescriptionWindow()
            {
                Title = this.Title,
                Message = Description
            };
        }
    }
    public KeyObjectStruct KeyObjectStruct
    {
        get
        {
            return new KeyObjectStruct()
            {
                Name = name,
                Description = this.DescriptionInfo,
                Track = this.Track,
            };
        }
    }

    private void Awake()
    {
        isSelected = false;
        isIndicated = false;
        isClickable = true;
        isMouseDownWithoutMoving = false;
        InitializeMaterials();
        InitializeRenderer();
        InitializeDescription();
        InitializeParticleSystem();
        InitializeRigidbody();
    }

    protected virtual void InitializeMaterials()
    {
        var resources = ResourcesAsset.Instance;
        if (resources != null)
        {
            if (resources.SelectedMaterial == null)
            {
                Debug.LogError("Resources Asset has no MidIndicatorMaterial.");
            }
            else
            {
                SelectedMaterial = resources.SelectedMaterial;
            }

            if (resources.IndicatorMaterial == null)
            {
                Debug.LogError("Resources Asset has no HighIndicatorMaterial.");
            }
            else
            {
                IndicatorMaterial = resources.IndicatorMaterial;
            }
        }
        else
        {
            Debug.LogError("Resources Asset is missing in Resources folder.");
        }
    }
    private void InitializeRenderer()
    {
        renderers = new Dictionary<MeshRenderer, Material>();

        if (TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderers.Add(renderer, renderer.material);
        }

        foreach (var childRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (!renderers.ContainsKey(childRenderer))
            {
                renderers.Add(childRenderer, childRenderer.material);

            }
        }
    }
    protected virtual void InitializeDescription()
    {
        if (canvas2D == null)
        {
            var canvasObject = Utility.GetCanvas2D();
            if (canvasObject != null)
            {
                if (!canvasObject.TryGetComponent(out canvas2D))
                {
                    Debug.LogError("Canvas2D has no CanvasController component.");
                }
            }
        }

        if (string.IsNullOrEmpty(Description) || canvas2D == null)
        {
            isClickable = false;
        }
    }
    private void InitializeParticleSystem()
    {
        if (!TryGetComponent<ParticleSystem>(out particles))
        {
            particles = GetComponentInChildren<ParticleSystem>();
        }
    }
    private void InitializeRigidbody()
    {
        if (TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }   

    public void Rotate()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime);
        gameObject.GetComponent<Renderer>().material.color = Color.green;
    }
    public virtual void SelectedMaterialOn()
    {
        isSelected = true;
        foreach (var render in renderers)
        {
            if (render.Key != null)
            {
                render.Key.material = SelectedMaterial;
            }
        }
    }    
    public virtual void SelectedMaterialOff()
    {
        isSelected = false;
        if (isIndicated)
        {
            IndicatorMaterialOn();
        }
        else
        {
            foreach (var render in renderers)
            {
                if (render.Key != null)
                {
                    render.Key.material = render.Value;
                }
            }
        }
    }
    public virtual void IndicatorMaterialOn()
    {
        isIndicated = true;
        foreach (var render in renderers)
        {
            if (render.Key != null)
            {
                render.Key.material = IndicatorMaterial;
            }
        }
    }
    public virtual void IndicatorMaterialOff()
    {
        isIndicated = false;

        if (isSelected)
        {
            SelectedMaterialOn();
        }
        else
        {
            foreach (var render in renderers)
            {
                if (render.Key != null)
                {
                    render.Key.material = render.Value;
                }
            }
        }
    }
    public virtual void ShowDescription()
    {
        if (canvas2D != null && ownWindowDescription == null)
        {
            ownWindowDescription = canvas2D.InstantiateDescriptionWindow();
            if (ownWindowDescription.TryGetComponent<DescriptionController>(out var controller))
            {
                controller.Initialize(Description, Title, null, null, SelectedMaterialOff);
            }
        }
    }
    public void PlayEffects()
    {
        if (particles != null)
        {
            particles.Play();
        }
    }
    public void StopEffects()
    {
        if (particles != null)
        {
            particles.Stop();
        }
    }
    public void StopNClearEffects()
    {
        if (particles != null)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    public void EnableRootRenderer()
    {
        if (TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer.enabled = true;
        }
    }
    public void DisableRootRenderer()
    {
        if (TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer.enabled = false;
        }
    }
    public void RotateValveToOpen()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateSelf(rotationValue));
    }
    public void RotateValveToClose()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateSelf(-rotationValue));
    }
    public void RotateValveToPartOpen()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateSelf(rotationValue * 0.5f));
    }
    public void RotateValveToPartClose()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateSelf(-rotationValue * 0.5f));
    }
    public void EnableClickable()
    {
        if (canvas2D != null && !string.IsNullOrEmpty(Description))
        {
            isClickable = true;
        }
    }
    public void DisableClickable()
    {
        isClickable = false;
    }
    public void DebugStart()
    {
        Debug.Log("Start: " + transform.name);
    }
    public void DebugEnd()
    {
        Debug.Log("End: " + transform.name);
    }

    public void CreateViewPoint(Vector3 position, Quaternion rotation, Transform parent)
    {
        var newPoint = GameObject.Find(name + ' ' + viewPositionName);

        if (newPoint == null)
        {
            newPoint = new GameObject(name + ' ' + viewPositionName);
        }

        newPoint.transform.position = position;
        newPoint.transform.rotation = rotation;
        newPoint.transform.parent = parent;
        ViewTransform = newPoint.transform;
    }
    public void SetDescriptionInfo(KeyObjectDescriptionWindow info)
    {
        if (!string.IsNullOrEmpty(info.Title))
        {
            Title = info.Title;
        }

        if (!string.IsNullOrEmpty(info.Message))
        {
            Description = info.Message;
        }
    }
    public virtual void SetClickable(bool value)
    {
        isClickable = value && !string.IsNullOrEmpty(Description);
    }

    private IEnumerator RotateSelf(float degrees)
    {
        var currentDegrees = 0.0f;
        var floatStep = degrees >= 0 ? -rotationSpeed : rotationSpeed;
        while (currentDegrees < Mathf.Abs(degrees))
        {
            float step = floatStep * Time.deltaTime;
            transform.Rotate(Vector3.up, step);
            currentDegrees += Mathf.Abs(step);
            yield return new WaitForEndOfFrame();
        }

        currentCoroutine = null;
    }

    private void OnMouseEnter()
    {
        if (isClickable && MouseController.IsActive && !EventSystem.current.IsPointerOverGameObject() && ownWindowDescription == null && !isSelected)
        {
            IndicatorMaterialOn();
        }
    }
    private void OnMouseExit()
    {
        if (isClickable && (MouseController.IsActive || isIndicated))
        {
            IndicatorMaterialOff();
        }
    }
    private void OnMouseDown()
    {
        if (isClickable && MouseController.IsActive && isIndicated && !EventSystem.current.IsPointerOverGameObject())
        {
            isMouseDownWithoutMoving = true;
            mousePositionOnDown = Input.mousePosition;
        }
    }
    private void OnMouseDrag()
    {
        if (isMouseDownWithoutMoving && mousePositionOnDown != Input.mousePosition)
        {
            isMouseDownWithoutMoving = false;
            if (isClickable && (MouseController.IsActive || isIndicated))
            {
                IndicatorMaterialOff();
            }
        }
    }
    private void OnMouseUpAsButton()
    {
        if (isClickable && isMouseDownWithoutMoving && isIndicated && !EventSystem.current.IsPointerOverGameObject() && !isSelected)
        {
            ShowDescription();
            SelectedMaterialOn();
            isMouseDownWithoutMoving = false;
        }
    }
}

[Serializable]
public struct KeyObjectStruct
{
    public string Name;
    public KeyObjectDescriptionWindow Description;
    public Track Track;
}

[Serializable]
public struct KeyObjectDescriptionWindow
{
    public string Title;
    public string Message;
}

[Serializable]
public struct Track
{
    public Vector3 ViewPosition;
    public Quaternion ViewRotation;
}


