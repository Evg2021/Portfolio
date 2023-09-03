using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TemplatePanelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public const string templateTypesName = "TemplateTypes";
    private const string trenObjectPlaceName = "TrenObjectPlace";
    public const string templateCameraName = "TemplateCamera";
    private const string canvasName = "Canvas";

    private const int renderTextureResolution = 1024;

    [SerializeField]
    private TextMeshProUGUI header;

    [SerializeField]
    private TemplateViewController templateViewPrefab;

    [SerializeField]
    private float distanceBetweenViews = 10.0f;

    private Vector2 startPosition;
    private bool allowToMoveWindow = false;

    private TemplateTypes templates;
    private ImageViewController imageViewController;
    private new Camera camera;
    private Transform trenObjectPlace;
    private Canvas canvas;    

    private bool isPressed = false;
    private bool isCursorInsideImage = false;

    private Rect imageRect;

    private TrenObjectControllerBase currentController;
    public string currentTrenObjectName;

    private TemplateViewController currentTemplateView;
    private RenderTexture currentRenderTexture;

    public void Initialize(string trenObjectName, string templateTypeName)
    {
        InitializeTemplateView();
        InitializeImageViewController();
        InitializeTemplateCamera();
        InitializeCanvas();
        InitializeTemplateTypes();

        InitializeFields(trenObjectName, templateTypeName);
    }
    private void InitializeTemplateView()
    {
        if (templateViewPrefab)
        {
            float maxHeight = -distanceBetweenViews;
            var views = FindObjectsOfType<TemplateViewController>();
            for (int i = 0; i < views.Length; i++)
            {
                if (views[i].transform.position.y > maxHeight)
                {
                    maxHeight = views[i].transform.position.y;
                }
            }

            currentTemplateView = Instantiate(templateViewPrefab);
            currentTemplateView.transform.position += Vector3.up * (maxHeight + distanceBetweenViews);
            currentTemplateView.Initialize();

            currentRenderTexture = new RenderTexture(renderTextureResolution, renderTextureResolution, 16, RenderTextureFormat.ARGB32);
            currentRenderTexture.Create();
        }
    }
    private void InitializeImageViewController()
    {
        if (currentTemplateView)
        {
            imageViewController = GetComponentInChildren<ImageViewController>();
            trenObjectPlace = currentTemplateView.transform.Find(trenObjectPlaceName)?.transform;
            if (imageViewController && trenObjectPlace)
            {
                imageViewController.Initialize(trenObjectPlace.transform);

                imageViewController.OnPressDown += ImageViewController_OnPressDown;
                imageViewController.OnPressUp += ImageViewController_OnPressUp;
                imageViewController.OnCursorEnter += ImageViewController_OnCursorEnter;
                imageViewController.OnCursorExit += ImageViewController_OnCursorExit;

                if (imageViewController.TryGetComponent<RawImage>(out var image))
                {
                    imageRect = image.rectTransform.rect;

                    if (currentRenderTexture)
                    {
                        image.texture = currentRenderTexture;
                    }
                }
            }
        }
    }
    private void InitializeTemplateCamera()
    {
        if (currentTemplateView)
        {
            var cameraGameObject = currentTemplateView.transform.Find(templateCameraName);
            if (cameraGameObject)
            {
                camera = cameraGameObject.GetComponent<Camera>();
                if (camera)
                {
                    camera.targetTexture = currentRenderTexture;
                }
            }
        }
    }
    private void InitializeCanvas()
    {
        canvas = GameObject.Find(UIController.MainCanvasName)?.GetComponent<Canvas>();
    }
    private void InitializeTemplateTypes()
    {
        templates = Resources.Load<TemplateTypes>(templateTypesName);
    }
    private void InitializeFields(string trenObjectName, string templateTypeName)
    {
        currentTrenObjectName = trenObjectName;

        if (header)
        {
            header.text = trenObjectName;
        }

        InstantiateTrenObject(templateTypeName);
    }

    private void ImageViewController_OnCursorExit()
    {
        isCursorInsideImage = false;
    }

    private void ImageViewController_OnCursorEnter()
    {
        isCursorInsideImage = true;
    }

    private void ImageViewController_OnPressUp()
    {
        isPressed = false;
    }

    private void ImageViewController_OnPressDown()
    {
        isPressed = true;
    }

    private void Update()
    {
        if (allowToMoveWindow && InputBase.Instance)
        {
            transform.position = startPosition + InputBase.Instance.DiffCursorPosition();
        }

        imageViewController.Rotate(InputBase.Instance.DiffCursorPosition() / Screen.dpi);

        if (isCursorInsideImage && camera)
        {
            var interactable = GetInteractableTrenObject();
            if (interactable != null)
            {
                if (currentController != interactable)
                {
                    currentController?.Disactivate();
                    currentController = interactable;
                    currentController.Activate();
                }
            }
            else
            {
                currentController?.Disactivate();
                currentController = null;
            }
        }
    }

    public void ShowPanel(string trenObjectName, string templateTypeName)
    {
        gameObject.SetActive(true);

        currentTrenObjectName = trenObjectName;

        if (header)
        {
            header.text = trenObjectName;
        }

        InstantiateTrenObject(templateTypeName);
    }

    public void HidePanel()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);

            currentTrenObjectName = string.Empty;

            if (header)
            {
                header.text = string.Empty;
            }

            RemoveTrenObjectFromPlace();
        }
    }

    public void ClosePanel()
    {
        if (currentTemplateView)
        {
            Destroy(currentTemplateView.gameObject);
        }

        if (currentRenderTexture)
        {
            Destroy(currentRenderTexture);
        }

        UIController.Instance.HideTemplateMenu(currentTrenObjectName);
    }

    private void InstantiateTrenObject(string templateName)
    {
        if (templates && trenObjectPlace && TrenObjectsManager.RegistratedTrenObjects != null)
        {
            var template = templates.GetTemplate(templateName);
            if (template)
            {
                var generatedTemplate = Instantiate(template, trenObjectPlace);
                var controllers = generatedTemplate.GetComponentsInChildren<TrenObjectControllerBase>();
                foreach (var controller in controllers)
                {
                    var registredObject = TrenObjectsManager.RegistratedTrenObjects.FirstOrDefault(h => h.Value.TrenObjectName == currentTrenObjectName &&
                                                                                                   h.Value.TrenObjectParameter == controller.ParameterName);
                    if (registredObject.Value != null)
                    {
                        if (registredObject.Value.TrenObjectParameter == controller.ParameterName)
                        {
                            unsafe
                            {
                                fixed (float* getValue = &registredObject.Value.pointValueGet, setValue = &registredObject.Value.pointValueSet)
                                {
                                    controller.Initialize(registredObject.Key, getValue, setValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (currentTemplateView)
        {
            currentTemplateView.InitializeTemplateCanvas();
        }
    }

    private void RemoveTrenObjectFromPlace()
    {
        if (trenObjectPlace && trenObjectPlace.childCount > 0)
        {
            Destroy(trenObjectPlace.GetChild(0).gameObject);
        }
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

    private TrenObjectControllerBase GetInteractableTrenObject()
    {
        if (canvas)
        {
            var scaleFactor = canvas.scaleFactor;
            var imageExtends = imageRect.size * scaleFactor * 0.5f;
            var mousePosition = InputBase.Instance.CursorPosition() - (transform.position.GetXY() - imageExtends);
            var uvMousePosition = new Vector3(mousePosition.x / (imageRect.size.x * scaleFactor),
                                              mousePosition.y / (imageRect.size.y * scaleFactor),
                                              0.0f);

            var ray = camera.ViewportPointToRay(uvMousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                return hit.transform.GetComponent<TrenObjectControllerBase>();
            }
        }

        return null;
    }

    private void OnDestroy()
    {
        if (imageViewController)
        {
            imageViewController.OnPressDown   -= ImageViewController_OnPressDown;
            imageViewController.OnPressUp     -= ImageViewController_OnPressUp;
            imageViewController.OnCursorEnter -= ImageViewController_OnCursorEnter;
            imageViewController.OnCursorExit  -= ImageViewController_OnCursorExit;
        }
    }
}
