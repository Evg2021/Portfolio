using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

public class PanoramaController : SingletonMonoBehaviour<PanoramaController>
{
    public static string PanoramsDataKeyToSave = "panorams_data";
    public static string BindingsDataKeyToSave = "bindings_data";

    public event Action<Panorama, ArrowInfo> TransitionAdded;
    public event Action<Panorama, ArrowInfo> TransitionRemoved;

    [SerializeField]
    private Material MainPanorama;

    public ResourcesPanorams PanoramsDataAsset;

    public Panorama currentPanorama { get; private set; }
    /// <summary>
    /// Key - name of panorama.
    /// </summary>
    public Dictionary<string, Panorama> panorams { get; private set; }

    private List<Arrow> currentPanoramaArrows;
    private List<TrenObject> currentPanoramaTrenObjects;
    private ArrowsTypesList arrowsTypes;
    private GameObject valveObject;

    private string arrowsListName = "ArrowsTypesList";
    private string valveObjectName = "ValveObject";
    private string dataExpansion = "json";

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        panorams = new Dictionary<string, Panorama>();
        arrowsTypes = Resources.Load<ArrowsTypesList>(arrowsListName);
        valveObject = Resources.Load<GameObject>(valveObjectName);
    }

    public TrenObject InstantiateTrenObject(Vector2 mousePosition)
    {
        var position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, valveObject.transform.position.z));

        var valve = Instantiate(valveObject, position, Quaternion.identity);
        valve.transform.LookAt(Camera.main.transform);

        if(valve.TryGetComponent<TrenObject>(out var component))
        {
            component.Initialize(Utilities.GetNameFromPath(currentPanorama.ImageLink));
            component.EnableMoving();
            return component;
        }

        return null;
    }

    public void HideArrows()
    {
        if(currentPanoramaArrows != null)
        {
            foreach(var arrow in currentPanoramaArrows)
            {
                arrow.gameObject.SetActive(false);
            }
        }
    }

    public void ShowArrows()
    {
        if (currentPanoramaArrows != null)
        {
            foreach (var arrow in currentPanoramaArrows)
            {
                arrow.gameObject.SetActive(true);
            }
        }
    }

    public Panorama GetPanorama(string path)
    {
        var splited = path.Split('\\');
        var keyName = splited[splited.Length - 1];

        Panorama panorama = null;

        if (panorams != null)
        {
            if (panorams.TryGetValue(keyName, out panorama))
            {
                if (panorama.ImageLink == path)
                {
                    return panorama;
                }
                else
                {
                    Debug.LogWarning("Panorama with the same name was added.");
                }
            }
            else
            {
                if (CreateNewPanorama(path, out panorama))
                {
                    return panorama;
                }
                else
                {
                    Debug.LogError("Adding panorama was failed.");
                }
            }
        }

        return panorama;
    }

    public void SetPanorams(Dictionary<string, Panorama> data)
    {
        if (data != null && data.Count > 0)
        {
            panorams = data;
            ChangePanorama(panorams.LastOrDefault().Value);
        }
        else
        {
            Debug.LogError("Read wrong panorama data.");
        }
    }

    public void SavePanoramsData()
    {
        //var mapData = MapController.Instance.GetMapItemsData();
        //var mapLayoutImage = MapController.Instance.MapLayoutImagePath;
        var data = Utilities.GetPanoramaDataJson(panorams);//, mapData, mapLayoutImage);
        Utilities.SaveBindingsData(new Bindings(panorams));
        if (Utilities.SavePanoramsData(data, dataExpansion))
        {
            Debug.Log("Data saved successfully.");
        }

        SaveToLastData(data);

        //Utilities.SavePanoramaDataInAsset(ref PanoramsDataAsset, panorams, null, null, true);
    }

    public void SaveToLastData(string data = null)
    {
        if (data == null || data.Length == 0)
        {
            //var mapData = MapController.Instance.GetMapItemsData();
            //var mapLayoutImage = MapController.Instance.MapLayoutImagePath;
            data = Utilities.GetPanoramaDataJson(panorams);//, mapData, mapLayoutImage);
        }

        PlayerPrefs.SetString(PanoramsDataKeyToSave, data);

        PlayerPrefs.SetString(BindingsDataKeyToSave, JsonUtility.ToJson(new Bindings(panorams)));
    }

    public void AddPanorama()
    {
        var path = Utilities.OpenFilePanel("jpg");
        var panorama = GetPanorama(path);
        if (panorama != null)
            ChangePanorama(panorama);
    }

    public bool CreateNewPanorama(string imageFilename, out Panorama panorama)
    {
        if (imageFilename == null || imageFilename.Length == 0)
        {
            panorama = null;
            return false;
        }

        var splitedFilename = imageFilename.Split('\\');
        var keyName = splitedFilename[splitedFilename.Length - 1];

        if (imageFilename.Length > 0)
        {
            panorama = new Panorama(imageFilename);

            panorams.Add(keyName, panorama);
        }
        else
        {
            panorama = null;
            Debug.LogError("Path to panorama is wrong.");
            return false;
        }

        return true;
    }

    public void MoveToPanorama(string path)
    {
        var panorama = GetPanorama(path);
        ChangePanorama(panorama);
    }

    public void ChangePanorama(Panorama panorama)
    {
        UpdateData();

        currentPanorama = panorama;
        if (panorama.ArrowsInfo != null)
        {
            InitializeArrows(panorama.ArrowsInfo);
            InitializeTrenObjects(panorama.TrenObjectsInfo);
        }

        var texture = Utilities.LoadTexture(panorama.ImageLink);
        Destroy(MainPanorama.GetTexture("_MainTex"));
        MainPanorama.SetTexture("_MainTex", texture);
    }

    public void ClearData()
    {
        panorams = new Dictionary<string, Panorama>();
        currentPanoramaArrows = new List<Arrow>();
        currentPanoramaTrenObjects = new List<TrenObject>();

        currentPanorama = null;
    }

    public void RemoveArrow(Arrow arrow)
    {
        if(currentPanoramaArrows.Contains(arrow))
        {
            currentPanoramaArrows.Remove(arrow);
            if (currentPanorama.ArrowsInfo != null)
            {
                var arrowInfo = currentPanorama.ArrowsInfo.FirstOrDefault(h => h.NextPanoramaFilename == arrow.NextPanorama.ImageLink);
                if (arrowInfo.NextPanoramaFilename != null && arrowInfo.NextPanoramaFilename.Length > 0)
                {
                    currentPanorama.ArrowsInfo.Remove(arrowInfo);
                    TransitionRemoved?.Invoke(currentPanorama, arrowInfo);
                }
            };
        }
    }

    public ArrowInfo GetReverseArrow(string currentPanoramaPath, ArrowInfo arrow)
    {
        if (arrow.NextPanoramaFilename != null && arrow.NextPanoramaFilename.Length > 0)
        {
            if (panorams.TryGetValue(Utilities.GetNameFromPath(arrow.NextPanoramaFilename), out var nextPanorama))
            {
                var nextArrows = nextPanorama.ArrowsInfo;
                if (nextArrows.Count > 0)
                {
                    var reverseArrow = nextArrows.Where(h => h.NextPanoramaFilename == currentPanoramaPath).FirstOrDefault();
                    if (reverseArrow != null)
                    {
                        return reverseArrow;
                    }
                }
            }
        }

        return null;
    }

    private void UpdateData()
    {
        if (currentPanorama == null || currentPanoramaArrows == null) return;

        var arrowsInfo = new List<ArrowInfo>();
        for (int i = 0; i < currentPanoramaArrows.Count; i++)
        {
            var info = currentPanoramaArrows[i].ArrowType == ArrowType.STAIRS ? GetStairsInfo(currentPanoramaArrows[i]) : GetArrowInfo(currentPanoramaArrows[i]);
            if (i < arrowsInfo.Count)
            {
                arrowsInfo[i] = info;
            }
            else
            {
                arrowsInfo.Add(info);
            }
        }

        currentPanorama.ArrowsInfo = arrowsInfo;

        UpdateBindingsData();

        SaveToLastData();
    }
    private void UpdateBindingsData()
    {
        if (currentPanoramaTrenObjects != null)
        {
            var trenObjectsInfo = new List<TrenObjectInfo>();
            foreach (var valve in currentPanoramaTrenObjects)
            {
                var info = valve.GetTrenObjectInfo();
                trenObjectsInfo.Add(info);
            }
            currentPanorama.TrenObjectsInfo = trenObjectsInfo;
        }
    }

    private void InitializeArrows(List<ArrowInfo> newArrowsInfo)
    {
        var oldArrows = FindObjectsOfType<Arrow>();
        foreach(var oldArrow in oldArrows)
        {
            Destroy(oldArrow.gameObject);
        }

        var newArrows = new List<Arrow>();
        if (arrowsTypes != null)
        {
            foreach (var info in newArrowsInfo)
            {
                if (info.NextPanoramaFilename == null || info.NextPanoramaFilename.Length == 0) continue;

                var panorama = GetPanorama(info.NextPanoramaFilename);
                if (panorama == null) continue;

                var arrowObject = Instantiate(arrowsTypes.GetArrow(info.ArrowType), Vector3.zero, info.Rotation);

                var childArrowObject = arrowObject.transform.GetChild(0);
                if(childArrowObject != null)
                {
                    childArrowObject.localPosition = info.Position;
                }

                if(arrowObject.TryGetComponent<Arrow>(out var component))
                {
                    if(panorama != null)
                    {
                        component.SetType((ArrowType)info.ArrowType);
                        component.SetNextPanorama(panorama, info.NextPanoramaEulers);

                        newArrows.Add(component);
                    }
                    else
                    {
                        Debug.LogError("Error with initialize arrow with next panorama (path:" + info.NextPanoramaFilename + ") on panorama (path:" + currentPanorama.ImageLink + ").");
                    }
                }

                if(info.ArrowType == (int)ArrowType.STAIRS)
                {
                    if(arrowObject.TryGetComponent<Stairs>(out var stairs))
                    {
                        stairs.stairsIsUp = ((StairsInfo)info).StairsIsUp;
                    }
                }
            }
        }

        currentPanoramaArrows = newArrows;
    }

    private void InitializeTrenObjects(List<TrenObjectInfo> trenObjectsInfo)
    {
        var oldTrenObjects = FindObjectsOfType<TrenObject>();
        foreach (var oldTrenObject in oldTrenObjects)
        {
            Destroy(oldTrenObject.gameObject);
        }

        currentPanoramaTrenObjects = new List<TrenObject>();

        if (trenObjectsInfo != null && trenObjectsInfo.Count > 0)
        {
            foreach (var info in trenObjectsInfo)
            {
                var trenObject = Instantiate(valveObject, info.Position, info.Rotation);
                if (trenObject.TryGetComponent<TrenObject>(out var component))
                {
                    component.Initialize(info.PanoramName, info.TrenObjectName, info.TemplateName);
                    currentPanoramaTrenObjects.Add(component);
                }
            }
        }
    }

    private void OnDisable()
    {
        if(currentPanorama != null)
        {
            SaveToLastData();
        }
    }

    public void InstantiateArrow(ArrowType arrowType, Vector3 cursorPosition)
    {
        var arrow = arrowsTypes.GetArrow(arrowType);

        if(arrow == null)
        {
            Debug.LogError("Arrow of type " + arrowType.ToString() + " is missing.");
        }

        var direction = Camera.main.transform.InverseTransformPoint(Camera.main.ScreenPointToRay(cursorPosition).direction);
        var angleY = Vector2.Angle(Vector2.up, new Vector2(direction.z, direction.y)) + Camera.main.transform.eulerAngles.x - 90.0f;
        var angleX = 90.0f - Vector2.Angle(Vector2.up, new Vector2(direction.z, direction.x));
        var rotation = Quaternion.Euler(Vector3.up * (CameraController.Instance.transform.eulerAngles.y + angleX));

        var arrowObject = Instantiate(arrow, arrow.transform.position, rotation);
        
        if(arrowObject.TryGetComponent<Arrow>(out var component))
        {
            var height = Mathf.Abs(component.LocalPosition.y);
            var minAngleLimit = Mathf.Atan(height / component.MaxDistance) * Mathf.Rad2Deg;
            var maxAngleLimit = Mathf.Atan(height / component.MinDistance) * Mathf.Rad2Deg;
            angleY = Mathf.Clamp(angleY, minAngleLimit, maxAngleLimit);
            var distance = height / Mathf.Tan(angleY * Mathf.Deg2Rad);
            component.LocalPosition = new Vector3(component.LocalPosition.x, component.LocalPosition.y, distance);

            component.SetType(arrowType);
            component.AllowMove();
        }
        else
        {
            Debug.LogError("Arrow object is broken. Check Arrow component on parent of arrow object.");
        }
    }

    private void AddArrowInPanoramaData(Arrow arrow)
    {
        ArrowInfo arrowInfo;

        if(arrow.ArrowType == ArrowType.STAIRS)
        {
            arrowInfo = GetStairsInfo(arrow);
        }
        else
        {
            arrowInfo = GetArrowInfo(arrow);
        }

        currentPanorama.AddArrow(arrowInfo);
        TransitionAdded?.Invoke(currentPanorama, arrowInfo);
    }

    private ArrowInfo GetArrowInfo(Arrow arrow)
    {
        var arrowInfo = new ArrowInfo();

        arrowInfo.Position = arrow.LocalPosition;
        arrowInfo.Rotation = arrow.Rotation;
        arrowInfo.ArrowType = (int)arrow.ArrowType;
        arrowInfo.NextPanoramaFilename = arrow.NextPanorama.ImageLink;
        arrowInfo.NextPanoramaEulers = arrow.NextPanoramaEulerAngles;

        return arrowInfo;
    }

    private StairsInfo GetStairsInfo(Arrow arrow)
    {
        if (arrow.GetType() != typeof(Stairs)) return null;

        var stairsInfo = new StairsInfo();
        stairsInfo.ArrowInfo = GetArrowInfo(arrow);
        stairsInfo.StairsIsUp = ((Stairs)arrow).IsStairsUp;

        return stairsInfo;
    }

    public void AddReverseArrow(Panorama panorama, Arrow arrow)
    {
        var cameraController = CameraController.Instance;

        var reverseArrowRotation = Quaternion.Euler(Vector3.up * (arrow.NextPanoramaEulerAngles.y - 180.0f));
        var reversePanoramaRotation = Vector3.zero;


        if (cameraController != null)
        {
            reversePanoramaRotation = new Vector3(cameraController.transform.localEulerAngles.x,
                                                  cameraController.transform.localEulerAngles.y - 180.0f,
                                                  cameraController.transform.localEulerAngles.z);
        }


        ArrowInfo reverseArrow;

        if(arrow.ArrowType == ArrowType.STAIRS)
        {
            reverseArrow = GetStairsInfo(arrow);
            bool stairsIsUpReverse = !((StairsInfo)reverseArrow).StairsIsUp;
            ((StairsInfo)reverseArrow).StairsIsUp = stairsIsUpReverse;
            //MapController.Instance.CurrentLevel += !stairsIsUpReverse ? 1 : -1;
        }    
        else
        {
            reverseArrow = GetArrowInfo(arrow);
        }

        reverseArrow.Rotation = reverseArrowRotation;
        reverseArrow.NextPanoramaEulers = reversePanoramaRotation;
        reverseArrow.NextPanoramaFilename = currentPanorama.ImageLink;

        panorama.AddArrow(reverseArrow);

        TransitionAdded?.Invoke(panorama, reverseArrow);

        if(arrow.ArrowType == ArrowType.STAIRS)
        {
            //MapController.Instance.CurrentLevel += ((StairsInfo)reverseArrow).StairsIsUp ? 1 : -1;
        }
    }

    public void ApproveArrow(Arrow arrow, string nextPanoramaFilename, Vector3 nextEulers, bool addReverseArrow = false)
    {
        if (nextPanoramaFilename != null && nextPanoramaFilename.Length > 0)
        {
            var panorama = GetPanorama(nextPanoramaFilename);
            if (panorama != null)
            {
                arrow.SetNextPanorama(panorama, nextEulers);

                if (!currentPanoramaArrows.Where(h => h.NextPanorama == arrow.NextPanorama).Any())
                {
                    currentPanoramaArrows.Add(arrow);
                    AddArrowInPanoramaData(arrow);
                }
                else
                {
                    UpdateData();
                }

                if (addReverseArrow)
                {
                    AddReverseArrow(panorama, arrow);
                }
            }
        }
    }

    public void ApproveTrenObject(ITrenObject data)
    {
        if (data != default(ITrenObject))
        {
            if (data is TrenObject)
            {
                if (currentPanoramaTrenObjects == null)
                {
                    currentPanoramaTrenObjects = new List<TrenObject>();
                }

                if (!currentPanoramaTrenObjects.Contains(data as TrenObject))
                {
                    currentPanoramaTrenObjects.Add(data as TrenObject);
                }

                UpdateData();
            }
        }
    }

    public void RemoveTrenObject(TrenObject trenObject)
    {
        if (currentPanoramaTrenObjects != null && currentPanoramaTrenObjects.Contains(trenObject))
        {
            currentPanoramaTrenObjects.Remove(trenObject);
            UpdateBindingsData();
        }
    }

    private void OnGUI()
    {
        if (currentPanorama != null)
        {
            float width = 100.0f;
            float height = 20.0f;

            var labelPoint = new Vector2(Screen.width * 0.5f, height);

            var rect = new Rect(labelPoint.x, labelPoint.y, width, height);
            var name = Utilities.GetNameFromPath(currentPanorama.ImageLink);

            GUI.Label(rect, name);
        }
    }
}


