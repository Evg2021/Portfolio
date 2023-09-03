using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdapterPanelController : SingletonMonoBehaviour<AdapterPanelController>, IPointerDownHandler, IPointerUpHandler
{
    public Toggle CheckBoxAutoNextPanorama;
    public Toggle CheckBoxAddReverseArrow;
    public GameObject StairsToggles;

    private string currentPanoramaName;
    private InputBase input;

    private bool allowRotation;
    private bool stairsIsUp;
    private bool autoReverseArrow;
    private bool nextPanoramaWasChanged = false;
    private bool allowToMoveWindow = false;

    private ImageViewController view;
    private new Transform camera;
    private Material viewMaterial;
    private Vector2 startPosition;

    private string AdapterViewHolderName = "AdapterView";
    private string StairsUpToggleName = "StairsUp";

    private Action<string, Vector3, bool> AfterSubmittingNextPanorama;

    public void Initialize()
    {
        input = InputBase.Instance;

        var adapter = GameObject.Find(AdapterViewHolderName);

        if (adapter != null)
        {
            var cameraComponent = adapter.GetComponentInChildren<Camera>();
            if(cameraComponent != null)
            {
                camera = cameraComponent.transform;
            }
            else
            {
                Debug.LogError("Camera in AdapterView object was not found.");
            }

            var meshRenderer = adapter.GetComponentInChildren<MeshRenderer>();
            if(meshRenderer != null)
            {
                viewMaterial = meshRenderer.sharedMaterial;
            }
            else
            {
                Debug.LogError("Sphere view from AdapterView is missing.");
            }
        }
        else
        {
            Debug.LogError("AdapterView object is missing on scene.");
        }

        view = GetComponentInChildren<ImageViewController>();
        if (view != null && camera != null)
        {
            view.Initialize(camera);
        }

        autoReverseArrow = true;

        HideStairsToogles();
    }

    public void SetToogleAutoReverseArrowFromUI()
    {
        autoReverseArrow = CheckBoxAddReverseArrow.isOn;
    }

    public void SetToogleAutoReverseArrow(bool value)
    {
        var bufferValue = autoReverseArrow;
        CheckBoxAddReverseArrow.isOn = value;
        autoReverseArrow = bufferValue;
    }

    public bool SetPanorama(string image, Action<string, Vector3, bool> afterSubmitting)
    {
        currentPanoramaName = image;

        if (currentPanoramaName != null && currentPanoramaName.Length > 0)
        {
            ChangePanoramaImage(Utilities.LoadTexture(currentPanoramaName));
            AfterSubmittingNextPanorama = afterSubmitting;
            return true;
        }

        return false;
    }

    public bool SetPanorama(Action<string, Vector3, bool> afterSubmitting)
    {
        if (SetNextPanoramaPath())
        {
            if (currentPanoramaName != null && currentPanoramaName.Length > 0)
            {
                ChangePanoramaImage(Utilities.LoadTexture(currentPanoramaName));
                AfterSubmittingNextPanorama = afterSubmitting;
                return true;
            }
        }

        return false;
    }

    public bool ChangePanorama(string path, Action<string, Vector3, bool> afterSubmitting)
    {
        if(Utilities.FileExist(path))
        {
            currentPanoramaName = path;
            ChangePanoramaImage(Utilities.LoadTexture(currentPanoramaName));
            AfterSubmittingNextPanorama = afterSubmitting;
            return true;
        }

        return false;
    }

    private void ChangePanoramaImage(Texture texture)
    {
        if(viewMaterial != null)
        {
            Destroy(viewMaterial.GetTexture("_BaseMap"));
            viewMaterial.SetTexture("_BaseMap", texture);
            view.ResetRotation();
        }
        else
        {
            Debug.LogError("Material for sphere AdapterView is missing");
        }
    }

    private bool SetNextPanoramaPath()
    {
        if (CheckBoxAutoNextPanorama.isOn && currentPanoramaName != null && currentPanoramaName.Length > 0)
        {
            string path = null;

            if(!nextPanoramaWasChanged)
                path = Utilities.GetNextImage(currentPanoramaName);

            if(path == null || path.Length == 0 || path == PanoramaController.Instance.currentPanorama.ImageLink)
            {
                path = Utilities.OpenFilePanel("jpg");
            }

            if (path != null && path.Length > 0)
                currentPanoramaName = path;
            else
            {
                return false;
            }
        }
        else
        {
            var path = Utilities.OpenFilePanel("jpg");
            if (path != null && path.Length > 0)
            {
                currentPanoramaName = path;
            }
            else
            {
                return false;
            }
        }

        if (currentPanoramaName != null && currentPanoramaName.Length > 0 && currentPanoramaName != PanoramaController.Instance.currentPanorama.ImageLink)
        {
            nextPanoramaWasChanged = true;
            return true;
        }


        return false;
    }

    public void ShowStairsToogles()
    {
        if(StairsToggles != null)
        {
            StairsToggles.SetActive(true);
        }
    }

    public void HideStairsToogles()
    {
        if(StairsToggles != null)
        {
            StairsToggles.SetActive(false);
        }
    }

    public void SetStateStairs(bool value)
    {
        if(StairsToggles != null)
        {
            var toogle = StairsToggles.GetComponentsInChildren<Toggle>().Where(h => h.name == StairsUpToggleName).FirstOrDefault();
            toogle.isOn = value;
        }
    }

    public void SetStateStairsToggles(Toggle secondToggle)
    {
        secondToggle.SetIsOnWithoutNotify(!secondToggle.isOn);
    }

    public void SetStairsUpToggle(Toggle stairsUpToggle)
    {
        stairsIsUp = stairsUpToggle.isOn;
    }

    public bool IsStairsUp()
    {
        return stairsIsUp;
    }

    public void OnClickSubmit()
    {
        if (UIController.Instance.CurrentArrow != null)
        {
            if (UIController.Instance.CurrentArrow.TryGetComponent<Stairs>(out var stairs))
            {
                stairs.stairsIsUp = stairsIsUp;
            }
        }

        if (!string.IsNullOrEmpty(currentPanoramaName))
        {
            AfterSubmittingNextPanorama?.Invoke(currentPanoramaName, view.EulerAngles, CheckBoxAddReverseArrow.isOn);
            gameObject.SetActive(false);

            nextPanoramaWasChanged = false;
        }
        else
        {
            Debug.LogError("Path to next panorama is empty.");
            OnCancel();
        }
    }

    public void SetViewRotation(Vector3 angles)
    {
        view.EulerAngles = angles;
    }

    public void OnClickChangePanorama()
    {
        var path = Utilities.OpenFilePanel("jpg");

        if (path == null || path.Length == 0 || path == PanoramaController.Instance.currentPanorama.ImageLink) return;

        currentPanoramaName = path;
        ChangePanoramaImage(Utilities.LoadTexture(currentPanoramaName));
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        allowRotation = true;
        CheckBoxAddReverseArrow.isOn = autoReverseArrow;
    }

    private void OnDisable()
    {
        HideStairsToogles();
    }

    private void Update()
    {
        if (allowToMoveWindow)
        {
            transform.position = startPosition + input.DiffCursorPosition();
        }

        if(allowRotation && view != null && input.GetFirstKey())
            view.Rotate(input.DiffCursorPosition() / Screen.dpi);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        startPosition = transform.position;

        allowToMoveWindow = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        allowToMoveWindow = false;
    }
}
