using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : SingletonMonoBehaviour<UIController>
{
    public const string MainCanvasName = "Canvas";
    private const string localIPAddres = "127.0.0.1";
    private const string settingsPath = "\\Settings.txt";

    [Header("Menus:")]

    [SerializeField]
    private RectTransform AdapterPanel;

    [SerializeField]
    private RectTransform ContextMenu;

    [SerializeField]
    private ObjectMenuController ObjectMenu;

    [SerializeField]
    private MapController MapController;

    [SerializeField]
    private RectTransform ValveMenu;

    [SerializeField]
    private TemplatePanelController TemplateMenuPrefab;

    [Header("Buttons:")]

    [SerializeField]
    private GameObject CreateNewButton;

    [SerializeField]
    private GameObject LoadButton;

    [SerializeField]
    private GameObject LoadLastButton;

    [SerializeField]
    private GameObject MapButton;

    [SerializeField]
    private GameObject LoadAssetButton;

    [SerializeField]
    private GameObject MoveToButton;

    [Header("Map:")]

    [SerializeField]
    private GameObject MapUIContainer;

    [SerializeField]
    private GameObject NextLevelButton;

    [SerializeField]
    private GameObject PreviousLevelButton;

    [SerializeField]
    private Text LevelInfo;

    [SerializeField]
    private GameObject UpdateMapButton;

    [Space(10)]

    [SerializeField]
    private Text MapLoadingProgressBar;

    [HideInInspector]
    public GameObject CurrentArrow;

    [HideInInspector]
    public IInteractableObject CurrentInteractableObject;

    private Clicker clicker;
    private string levelInfoPrefix = "Уровень ";
    private string mapLoadingProgressBarTitle = "Загрузка карты: {0}%";

    private Dictionary<string, TemplatePanelController> templateViews = new Dictionary<string, TemplatePanelController>();

    private Canvas mainCanvas;

    public AdapterPanelController AdapterController
    {
        get
        {
            if (AdapterPanel != null)
                return AdapterPanel.GetComponent<AdapterPanelController>();
            return null;
        }
    }

    private InputBase input;
    private PanoramaController panoramaController;

    private void Start()
    {
        Initialize();
        HideMoveToButton();
        ShowMainButtons();
    }

    private void OnDisable()
    {
        input.SecondKeyIsDown -= Input_SecondKeyIsDown;
        input.FirstKeyIsDown -= Input_FirstKeyIsDown;

        var keyboard = (InputKeyboard)input;
        if (keyboard != null)
        {
            keyboard.HotKeyToOpenImage -= OnClickAddArrow;
            keyboard.HotKeyCancel      -= Cancel;
            keyboard.HotKeyApprove     -= Approve;
            keyboard.HotKeySave        -= OnClickSaveButton;
            keyboard.HotKeyLoad        -= OnClickLoadButton;
        }

        ClientSocketManager.ClearRegistration();
        ClientSocketManager.CloseConnect();
    }

    public void ShowObjectMenu(Vector2 position, IInteractableObject valve)
    {
        CurrentInteractableObject = valve;
        ObjectMenu.transform.position = new Vector3(position.x, position.y, ObjectMenu.transform.position.z);
        ObjectMenu.gameObject.SetActive(true);
        
        if (valve is ITrenObject)
        {
            ObjectMenu.SetCurrentTrenObject(valve as ITrenObject);
        }
    }

    public void OnClickRemoveObject()
    {
        CurrentInteractableObject?.Remove();
        if (!CameraController.Instance.IsActive)
        {
            CameraController.Instance.EnableControl();
        }
    }

    public void HideObjectMenu()
    {
        ObjectMenu.gameObject.SetActive(false);
    }

    private void Initialize()
    {
        Utilities.separator = Path.DirectorySeparatorChar;

        mainCanvas = GameObject.Find(MainCanvasName)?.GetComponent<Canvas>();

        input = InputBase.Instance;
        if (input == null)
            Debug.LogError("InputBase object was not found.");

        input.SecondKeyIsDown += Input_SecondKeyIsDown;
        input.FirstKeyIsDown += Input_FirstKeyIsDown;

        var keyboard = (InputKeyboard)input;
        if (keyboard != null)
        {
            keyboard.HotKeyToOpenImage += OnClickAddArrow;
            keyboard.HotKeyCancel      += Cancel;
            keyboard.HotKeyApprove     += Approve;
            keyboard.HotKeySave        += OnClickSaveButton;
            keyboard.HotKeyLoad        += OnClickLoadButton;
            keyboard.HotKeyDelete      += OnClickRemoveArrow;
        }

        panoramaController = PanoramaController.Instance;
        if (panoramaController == null)
            Debug.LogError("PanoramaController object was not found.");

        clicker = CameraController.Instance.transform.GetComponent<Clicker>();
        if(clicker == null)
            Debug.LogError("Clicker component on camera was not found.");

        if (MapController != null)
        {
            MapController.Initialize();

            if (MapController.gameObject.activeSelf)
                HideMap();
        }
        else
        {
            Debug.LogError("Map is missing in UIController.");
        }

        HideAllMenus();

        AdapterController.Initialize();

        ConnectToSimulator();
    }
    private void ConnectToSimulator()
    {
        string IPAddress = localIPAddres;
        string path = Application.streamingAssetsPath + settingsPath;
        if (File.Exists(path))
        {
            string[] IP = File.ReadAllLines(path);
            IPAddress = IP[0];
        }

        TrenObjectsManager.ConnectToSimulator(IPAddress);
    }

    private void HideAllMenus()
    {
        HideContextMenu();
        HideAdapterMenu();
        HideMapButton();
        HideObjectMenu();
        HideValveMenu();
        HideMapLoadingProgressBar();
        HideMoveToButton();
    }

    public void ShowTemplateMenu(string trenObjectName, string templateTypeName)
    {
        if (mainCanvas && TemplateMenuPrefab && !templateViews.ContainsKey(trenObjectName))
        {
            var newTempalte = Instantiate(TemplateMenuPrefab, mainCanvas.transform);
            newTempalte.Initialize(trenObjectName, templateTypeName);
            templateViews.Add(trenObjectName, newTempalte);   
        }
    }

    public void HideTemplateMenu(string trenObjectName)
    {
        if (templateViews != null && templateViews.TryGetValue(trenObjectName, out var panel))
        {
            Destroy(panel.gameObject);
            templateViews.Remove(trenObjectName);
        }
    }

    public void ShowValveMenu(Vector2 position, ValveObject valve)
    {
        if(ValveMenu != null)
        {
            ValveMenu.gameObject.SetActive(true);
        }
    }

    public void HideValveMenu()
    {
        if (ValveMenu != null)
        {
            ValveMenu.gameObject.SetActive(false);
        }
    }

    private void EnableClicker()
    {
        if(clicker != null)
        {
            clicker.Enable();
        }
    }

    private void DisableClicker()
    {
        if(clicker != null)
        {
            clicker.Disable();
        }
    }

    public void CheckMapUI()
    {
        if(MapController.MapUILevel == MapController.MaxLevel)
        {
            NextLevelButton.SetActive(false);
        }
        else if(!NextLevelButton.activeSelf && MapController.MapUILevel < MapController.MaxLevel)
        {
            NextLevelButton.SetActive(true);
        }

        if (MapController.MapUILevel == MapController.MinLevel)
        {
            PreviousLevelButton.SetActive(false);
        }
        else if (!PreviousLevelButton.activeSelf && MapController.MapUILevel > MapController.MinLevel)
        {
            PreviousLevelButton.SetActive(true);
        }

        LevelInfo.text = levelInfoPrefix + MapController.MapUILevel;
    }

    public void OnClickNextLevel()
    {
        if(MapController != null)
        {
            int nextIndex = MapController.MapUILevel + 1;
            if(nextIndex <= MapController.MaxLevel)
            {
                MapController.ShowLevel(nextIndex, false);
                CheckMapUI();
            }
        }
    }

    public void OnClickPreviousLevel()
    {
        if(MapController != null)
        {
            int nextIndex = MapController.MapUILevel - 1;
            if (nextIndex >= MapController.MinLevel)
            {
                MapController.ShowLevel(nextIndex, false);
                CheckMapUI();
            }
        }
    }

    private void HideMainButtons()
    {
        SetActiveMainButtons(false);
    }

    private void ShowMainButtons()
    {
        SetActiveMainButtons(true);
    }

    private void SetActiveMainButtons(bool value)
    {
        if (LoadButton != null)
        {
            LoadButton.SetActive(value);
        }

        if (CreateNewButton != null)
        {
            CreateNewButton.SetActive(value);
        }

        if (LoadLastButton != null)
        {
            if(value == true)
            {
                if(PlayerPrefs.HasKey(PanoramaController.PanoramsDataKeyToSave) && PlayerPrefs.GetString(PanoramaController.PanoramsDataKeyToSave).Length != 0)
                {
                    LoadLastButton.SetActive(value);
                }
            }
            else
            {
                LoadLastButton.SetActive(value);
            }
        }

        if(LoadAssetButton != null)
        {
            LoadAssetButton.SetActive(value);
        }
    }

    private void HideMapButton()
    {
        if (MapButton != null)
        {
            MapButton.SetActive(false);
        }
    }

    private void ShowMapButton()
    {
        if (MapButton != null)
        {
            MapButton.SetActive(true);
        }
    }

    private void HideMoveToButton()
    {
        if (MoveToButton != null)
        {
            MoveToButton.SetActive(false);
        }
    }

    private void ShowMoveToButton()
    {
        if (MoveToButton)
        {
            MoveToButton.SetActive(true);
        }
    }

    private void SetActiveMapUI(bool value)
    {
        if(MapUIContainer != null)
        {
            MapUIContainer.SetActive(value);
        }
    }

    public void OnClickUpdateMapButton()
    {
        var mapData = MapController.GetMapItemsData();
        var mapItemPositions = MapController.MapPositionsToDictionary(mapData);
        var mapItemLevels = MapController.MapLevelsToDictionary(mapData);

        MapController.UpdateMap(PanoramaController.Instance.panorams, mapItemPositions, mapItemLevels);
    }

    private void ShowMapUI()
    {
        SetActiveMapUI(true);
    }

    private void HideMapUI()
    {
        SetActiveMapUI(false);
    }

    private void Input_FirstKeyIsDown(Vector2 position)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (ObjectMenu.gameObject.activeSelf && CurrentInteractableObject != null && !CurrentInteractableObject.Approved())
            {
                CurrentInteractableObject.Remove();
                CurrentInteractableObject = null;

                if (!CameraController.Instance.IsActive)
                {
                    CameraController.Instance.EnableControl();
                }
            }

            HideContextMenu();
            HideObjectMenu();
            HideValveMenu();
        }
    }

    public void HideContextMenu()
    {
        if (ContextMenu == null)
        {
            Debug.LogError("ContextMenu object is missing.");
            return;
        }

        ContextMenu.gameObject.SetActive(false);
    }

    private void Input_SecondKeyIsDown(Vector2 position)
    {
        if (Clicker.EditorMode)
        {
            var ray = Camera.main.ScreenPointToRay(input.CursorPosition());

            if (!Physics.Raycast(ray))
            {
                if (panoramaController.currentPanorama != null && !EventSystem.current.IsPointerOverGameObject() && CameraController.Instance.IsActive)
                {
                    OpenContextMenu(position);
                }
            }
        }
    }

    private void OpenContextMenu(Vector2 position)
    {
        if(ContextMenu == null && /*!MapController.gameObject.activeSelf &&*/ !AdapterPanel.gameObject.activeSelf)
        {
            Debug.LogError("ContextMenu object is missing.");
            return;
        }

        ContextMenu.position = new Vector3(position.x, position.y, ContextMenu.transform.position.z);
        ContextMenu.gameObject.SetActive(true);
    }

    public void OpenAdapterMenu(bool openStairsOptions = false)
    {
        if(AdapterPanel == null)
        {
            Debug.LogError("AdapterPanel object is missing.");
            return;
        }

        AdapterPanel.gameObject.SetActive(true);

        if(openStairsOptions)
        {
            AdapterController.ShowStairsToogles();
        }
    }

    public void OnClickCloseAdpaterMenu()
    {
        HideAdapterMenu();

        if(CurrentArrow != null)
        {
            if (CurrentArrow.TryGetComponent<Arrow>(out var component))
            {
                if (component.NextPanorama == null || component.NextPanorama.ImageLink == null && component.NextPanorama.ImageLink.Length == 0)
                    RemoveArrow(CurrentArrow);
            }

        }
    }

    public void HideAdapterMenu()
    {
        if (AdapterPanel == null)
        {
            Debug.LogError("AdapterPanel object is missing.");
            return;
        }

        AdapterPanel.gameObject.SetActive(false);
    }

    public void OnClickCreateNewButton()
    {

        if (panoramaController.CreateNewPanorama(Utilities.OpenFilePanel("jpg"), out var newPanorama))
        {
            panoramaController.ClearData();
            panoramaController.ChangePanorama(newPanorama);
            HideMainButtons();
            ShowMoveToButton();

            //MapController.RemoveMap();
        }

        HideContextMenu();
    }

    public void OnClickOpenMapButton()
    {
        if (MapController != null)
        {
            if (!MapController.gameObject.activeSelf)
            {
                OpenMap();
            }
            else
            {
                HideMap();
            }
        }
    }

    public void OnClickChangeMapLayout()
    {
        var path = Utilities.OpenFilePanel("jpg");

        if(path != null && path.Length > 0)
            MapController.MapLayoutImagePath = path;
    }

    public void OnClickLoadFromAssetButton()
    {
        if(panoramaController.PanoramsDataAsset != null)
        {
            Utilities.ParseDataFromAsset(panoramaController.PanoramsDataAsset.Data, out var panorams, out var mapData, out string mapLayoutName);

            Dictionary<string, Vector3> map = null;
            Dictionary<string, int> levels = null;
            if (mapData != null)
            {
                map = MapController.MapPositionsToDictionary(mapData);
                levels = MapController.MapLevelsToDictionary(mapData);
            }

            if (mapLayoutName != null && mapLayoutName.Length > 0)
                MapController.MapLayoutImagePath = mapLayoutName;

            HideMainButtons();
            ShowMapLoadingProgressBar();

            StartCoroutine(MapController.UpdateMap(CheckLoadingProgress, delegate
            {
                panoramaController.SetPanorams(panorams);
                ShowMapButton();
                HideMapLoadingProgressBar();
            }
            , panorams, map, levels));
        }
    }

    public void OnClickLoadButton()
    {
        if (!AdapterPanel.gameObject.activeSelf)
        {
            if (Utilities.ReadData(out var panorams, out _, out _, "json"))
            {
                //var map = MapController.MapPositionsToDictionary(mapData);
                //var levels = MapController.MapLevelsToDictionary(mapData);

                /*if (mapLayoutName != null && mapLayoutName.Length > 0)
                    MapController.MapLayoutImagePath = mapLayoutName;*/

                HideMainButtons();
                panoramaController.SetPanorams(panorams);
                ShowMoveToButton();
                //ShowMapLoadingProgressBar();

                /*StartCoroutine(MapController.UpdateMap(CheckLoadingProgress, delegate
                {
                    panoramaController.SetPanorams(panorams);
                    ShowMapButton();
                    HideMapLoadingProgressBar();
                }
                , panorams, map, levels));*/
            }
        }
        else
        {
            AdapterController.OnClickChangePanorama();
        }

        HideContextMenu();
    }

    public void OnClickLoadLastButton()
    {
        var data = PlayerPrefs.GetString(PanoramaController.PanoramsDataKeyToSave);

        Utilities.ParseDataFromJson(data, out var panorams, out _, out var mapLayoutName);

        var bindingsData = PlayerPrefs.GetString(PanoramaController.BindingsDataKeyToSave);
        if (!string.IsNullOrEmpty(bindingsData))
        {
            try
            {
                var bindings = JsonUtility.FromJson<Bindings>(bindingsData);
                TrenObjectsManager.RegistrateObjects(bindings);
                Utilities.AddBindingsDataToPanorams(bindings, ref panorams);
            }
            catch (Exception ex)
            {
                Debug.LogError("Reading bindigs from PlayerPrefs error:" + ex.Message);
            }
        }

        if (panorams != null && panorams.Count > 0)
        {
            //var map = MapController.MapPositionsToDictionary(mapData);
            //var levels = MapController.MapLevelsToDictionary(mapData);

            /*if (mapLayoutName != null && mapLayoutName.Length > 0)
                MapController.MapLayoutImagePath = mapLayoutName;*/

            HideMainButtons();
            panoramaController.SetPanorams(panorams);
            ShowMoveToButton();
            //ShowMapLoadingProgressBar();

            /*StartCoroutine(MapController.UpdateMap(CheckLoadingProgress, delegate
            {
                panoramaController.SetPanorams(panorams);
                ShowMapButton();
                HideMapLoadingProgressBar();
            }
            , panorams, map, levels));*/
        }

        HideContextMenu();
    }

    public void OnClickValveCreation()
    {
        var currentValveObject = panoramaController.InstantiateTrenObject(input.CursorPosition());
        try
        {
            CurrentInteractableObject = (IInteractableObject)currentValveObject;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnClickMoveToButton()
    {
        if (panoramaController != null)
        {
            string path = Utilities.OpenFilePanel("jpg");
            if (!string.IsNullOrEmpty(path))
            {
                HideContextMenu();
                HideObjectMenu();
                HideValveMenu();

                if (AdapterPanel.gameObject.activeSelf)
                    AdapterController.OnCancel();

                panoramaController.MoveToPanorama(path);
            }
        }
        else
        {
            Debug.LogError("Panorama Controller is missing.");
        }
    }

    private void ShowMapLoadingProgressBar()
    {
        SetActiveMapLoadingProgressBar(true);
    }

    private void HideMapLoadingProgressBar()
    {
        SetActiveMapLoadingProgressBar(false);
    }

    private void SetActiveMapLoadingProgressBar(bool value)
    {
        if(MapLoadingProgressBar != null)
        {
            MapLoadingProgressBar.gameObject.SetActive(value);

            if(value == true)
            {
                MapLoadingProgressBar.text = string.Format(mapLoadingProgressBarTitle, 0.0f);
            }
        }
    }

    private void CheckLoadingProgress(float progress)
    {
        if(MapLoadingProgressBar != null)
        {
            MapLoadingProgressBar.text = string.Format(mapLoadingProgressBarTitle, (int)progress);
        }
    }

    public void OnClickSaveButton()
    {
        if(panoramaController.currentPanorama != null)
            panoramaController.SavePanoramsData();
        HideContextMenu();
    }

    public void OnClickSaveBindingsDataButton()
    {
        Utilities.SaveBindingsData(new Bindings(PanoramaController.Instance.panorams));
    }

    public void OnClickAddStairs()
    {
        AddArrow(ArrowType.STAIRS);
    }

    public void OnClickAddArrow()
    {
        AddArrow(ArrowType.ARROW);
    }

    private void AddArrow(ArrowType type)
    {
        if (panoramaController == null || AdapterPanel == null || panoramaController.currentPanorama == null) return;

        if (!AdapterPanel.gameObject.activeSelf && CameraController.Instance.IsActive)
        {
            panoramaController.InstantiateArrow(type, input.CursorPosition());
        }
    }

    public void OpenMap()
    {
        panoramaController.HideArrows();
        MapController.ShowLevel(MapController.MapUILevel, false);
        MapController.gameObject.SetActive(true);
        //var positions = MapController.ComputeMap(panoramaController.panorams);
        //MapController.ShowPanoramsMap(positions);
        //MapController.ShowTransitions(panoramaController, positions);

        ShowMapUI();
        CheckMapUI();
        CameraController.Instance.DisableControl();
    }

    public void HideMap()
    {
        //MapController.HidePanoramsMap();
        //MapController.HideTransitions();
        MapController.gameObject.SetActive(false);
        HideMapUI();

        panoramaController.ShowArrows();

        CameraController.Instance.EnableControl();
    }

    public void Cancel()
    {
        HideContextMenu();
        HideObjectMenu();
        HideValveMenu();
        
        if(AdapterPanel.gameObject.activeSelf)
            AdapterController.OnCancel();

        if (CurrentArrow != null)
        {
            if (CurrentArrow.TryGetComponent<Arrow>(out var component))
            {
                if (component.NextPanorama == null || component.NextPanorama.ImageLink == null && component.NextPanorama.ImageLink.Length == 0)
                {
                    RemoveArrow(CurrentArrow);
                }
            }
        }

        if (MapController != null && MapController.gameObject.activeSelf)
        {
            HideMap();
        }

        if (!CameraController.Instance.IsActive)
        {
            CameraController.Instance.EnableControl();
        }

        if (CurrentInteractableObject != null && !CurrentInteractableObject.Approved())
        {
            CurrentInteractableObject.Remove();
            CurrentInteractableObject = null;
        }

        clicker.Cancel();
    }

    public void OnClickRemoveArrow()
    {
        if (CurrentArrow != null)
            RemoveArrow(CurrentArrow);

        if (AdapterPanel.gameObject.activeSelf)
            HideAdapterMenu();

        if (!CameraController.Instance.IsActive)
            CameraController.Instance.EnableControl();
    }

    private void RemoveArrow(GameObject arrow)
    {
        if (arrow.TryGetComponent<Arrow>(out var component))
        {
            panoramaController.RemoveArrow(component);
        }
        Destroy(arrow);
    }

    public void Approve()
    {
        if(AdapterPanel.gameObject.activeSelf)
        {
            AdapterController.OnClickSubmit();
            CurrentArrow = null;
            return;
        }

        if(CurrentArrow != null)
        {
            if(CurrentArrow.TryGetComponent<Arrow>(out var arrow))
            {
                arrow.Approve();
            }
        }

        if(CreateNewButton != null && CreateNewButton.activeSelf)
        {
            OnClickCreateNewButton();
        }
    }
}
