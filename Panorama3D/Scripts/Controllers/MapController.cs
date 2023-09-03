using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : SingletonMonoBehaviour<MapController>
{
    public MapItem MapItem;
    public GameObject TransitionPrefab;
    public MeshRenderer MapLayoutRenderer;

    public bool AddArrows = true;

    public int LevelsCount
    {
        get
        {
            if (levels != null)
                return levels.Count;
            return 0;
        }
    }

    private string mapLayoutImagePath;
    public string MapLayoutImagePath
    {
        get
        {
            return mapLayoutImagePath;
        }
        set
        {
            mapLayoutImagePath = value;
            SetTextureToMapLayout(value);
        }
    }

    [HideInInspector]
    public int CurrentLevel;

    [HideInInspector]
    public int MapUILevel;

    public int MaxLevel { get; private set; }
    public int MinLevel { get; private set; }

    private InputBase input;
    private PanoramaController panoramaController;
    private new MeshRenderer renderer;
    private Dictionary<string, List<TransitionData>> transitions;
    private Dictionary<string, MapItem> panorams;
    private Dictionary<int, List<MapItem>> levels;

    private Vector3 startPosition;
    private Vector2 minPoint;
    private Vector2 maxPoint;
    private Vector3 transitionOffset;

    private float maxDistance = 150.0f;
    private float minDistance = 10.0f;
    private float distanceBetweenPanoramsKoeff = 0.2f;

    private bool allowToMove;

    private void SetBoarders()
    {
        maxPoint = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, maxDistance)));
        minPoint = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, maxDistance)));
    }

    public void SetTextureToMapLayout(string imagePath)
    {
        if (MapLayoutRenderer != null)
        {
            ShowMapLayout();
            MapLayoutRenderer.material.SetTexture("_EmissionMap", Utilities.LoadTexture(imagePath));
        }
            
    }

    public void HideMapLayout()
    {
        if (MapLayoutRenderer != null && MapLayoutRenderer.gameObject.activeSelf)
        {
            MapLayoutRenderer.gameObject.SetActive(false);
        }
    }

    public void ShowMapLayout()
    {
        if (MapLayoutRenderer != null && !MapLayoutRenderer.gameObject.activeSelf)
        {
            MapLayoutRenderer.gameObject.SetActive(true);
        }
    }

    public void Initialize()
    {
        InitializeInstance(this);

        input = InputBase.Instance;
        if (input == null)
            Debug.LogError("Input component is missing on scene.");

        panoramaController = PanoramaController.Instance;
        if (panoramaController == null)
            Debug.LogError("PanoramaController is missing on scene.");

        panoramaController.TransitionAdded += PanoramaController_TransitionAdded;
        panoramaController.TransitionRemoved += PanoramaController_TransitionRemoved;

        panorams = new Dictionary<string, MapItem>();
        transitions = new Dictionary<string, List<TransitionData>>();
        levels = new Dictionary<int, List<MapItem>>();
        CurrentLevel = 0;
        MapUILevel = 0;
        transitionOffset = Vector3.forward * (MapItem.transform.localScale.z + 1.0f) * 0.5f;
        MaxLevel = 0;
        MinLevel = 0;

        SetBoarders();
        HideMapLayout();

        EnableMoving();
    }

    public void EnableMoving()
    {
        allowToMove = true;
    }

    public void DisableMoving()
    {
        allowToMove = false;
    }

    private void RemoveMapItemFromLevels(int index, MapItem item)
    {
        if (levels != null)
        {
            if (levels.TryGetValue(index, out var items))
            {
                if (items.Contains(item))
                {
                    items.Remove(item);
                }
            }
        }

        if (levels[index].Count == 0)
        {
            levels.Remove(index);

            if (index == MinLevel)
            {
                MinLevel++;
            }
            else if (index == MaxLevel)
            {
                MaxLevel--;
            }
            else
            {
                List<int> indices = new List<int>();

                if (index < 0)
                {
                    int startIndex = MinLevel;
                    while (startIndex < index && levels.ContainsKey(startIndex))
                    {
                        indices.Add(startIndex);
                        startIndex++;
                    }

                    MinLevel++;
                }
                else if (index >= 0)
                {
                    int startIndex = MaxLevel;
                    while (startIndex > index && levels.ContainsKey(startIndex))
                    {
                        indices.Add(startIndex);
                        startIndex--;
                    }

                    MaxLevel--;
                }

                indices.Reverse();

                int stepSign = -index / Mathf.Abs(index);

                foreach (int oldIndex in indices)
                {
                    if (!levels.ChangeKey(oldIndex, oldIndex + stepSign))
                    {
                        Debug.LogError("Error with map squeezing");
                    }
                }
            }

            if (index == CurrentLevel)
            {
                ShowLevel(levels.First().Key);
            }
        }
    }

    private void RemoveHorizontalTransition(Panorama panorama, ArrowInfo arrow)
    {
        TransitionData transitionForRemove = default(TransitionData);
        if (transitions.TryGetValue(panorama.ImageLink, out var arrows))
        {
            transitionForRemove = arrows.Where(h => h.NextItem == arrow.NextPanoramaFilename).FirstOrDefault();
            if (!transitionForRemove.Equals(default(TransitionData)))
            {
                arrows.Remove(transitionForRemove);
                Destroy(transitionForRemove.TransitionObject);
                panorams[panorama.ImageLink].RemoveTransitionOutside(transitionForRemove.TransitionObject.transform);
                panorams[arrow.NextPanoramaFilename].RemoveTransitionInside(transitionForRemove.TransitionObject.transform);
            }

            if (arrows.Count == 0)
            {
                if (transitions.Where(h => h.Value.Where(v => v.NextItem == panorama.ImageLink).Any()).Any())
                {
                    var mapItemObject = panorams[panorama.ImageLink];

                    if (mapItemObject.Stairs.Count == 0)
                    {
                        panorams.Remove(panorama.ImageLink);
                        Destroy(mapItemObject.gameObject);

                        RemoveMapItemFromLevels(CurrentLevel, mapItemObject);
                    }
                }
            }
        }

        if (transitions.TryGetValue(arrow.NextPanoramaFilename, out var nextArrows))
        {
            if (nextArrows.Count == 0)
            {
                var mapItemObject = panorams[arrow.NextPanoramaFilename];

                if (mapItemObject.Stairs.Count == 0)
                {
                    panorams.Remove(panorama.ImageLink);
                    Destroy(mapItemObject.gameObject);

                    RemoveMapItemFromLevels(CurrentLevel, mapItemObject);
                }
            }
        }
    }

    private void RemoveVerticalTransition(Panorama panorama, StairsInfo stairs)
    {
        int nextIndex = stairs.StairsIsUp ? CurrentLevel + 1 : CurrentLevel - 1;

        if (levels.TryGetValue(nextIndex, out var mapItems))
        {
            var item = mapItems.Where(h => h.PanoramaPath == stairs.NextPanoramaFilename).FirstOrDefault();
            if (item != null)
            {
                var stairsToRemove = item.Stairs.Where(h => h.NextPanoramaFilename == stairs.NextPanoramaFilename).FirstOrDefault();
                if (stairsToRemove != null)
                {
                    item.Stairs.Remove(stairsToRemove);
                }

                if (item.Stairs.Count == 0)
                {
                    if (item.TransitionsInside.Count == 0 && item.TransitionsOutside.Count == 0)
                    {
                        RemoveMapItemFromLevels(nextIndex, item);

                        panorams.Remove(item.PanoramaPath);
                        Destroy(item.gameObject);

                        if (mapItems.Count == 0)
                        {
                            levels.Remove(nextIndex);
                        }
                    }
                }
            }

        }

        if (panorams.TryGetValue(panorama.ImageLink, out var startMapItem))
        {
            var stairsToRemove = startMapItem.Stairs.Where(h => h.NextPanoramaFilename == stairs.NextPanoramaFilename).FirstOrDefault();
            if (stairsToRemove != null)
            {
                startMapItem.Stairs.Remove(stairsToRemove);
            }

            if (startMapItem.Stairs.Count == 0)
            {
                if (startMapItem.TransitionsInside.Count == 0 && startMapItem.TransitionsOutside.Count == 0)
                {
                    RemoveMapItemFromLevels(CurrentLevel, startMapItem);
                    panorams.Remove(startMapItem.PanoramaPath);
                    Destroy(startMapItem.gameObject);

                    if (levels[CurrentLevel].Count == 0)
                    {
                        levels.Remove(CurrentLevel);
                        CurrentLevel++;

                        if (!levels.ContainsKey(CurrentLevel))
                        {
                            CurrentLevel -= 2;
                            if (!levels.ContainsKey(CurrentLevel))
                            {
                                CurrentLevel = 0;
                            }
                        }

                    }
                }
            }
        }
    }

    private void PanoramaController_TransitionRemoved(Panorama panorama, ArrowInfo arrow)
    {
        if (!AddArrows) return;

        if (arrow.ArrowType == (int)ArrowType.STAIRS)
        {
            RemoveVerticalTransition(panorama, (StairsInfo)arrow);
        }
        else
        {
            RemoveHorizontalTransition(panorama, arrow);
        }
    }

    public void UpdateMapItem(MapItem currentMapItem)
    {
        var transitionsInside = currentMapItem.TransitionsInside;
        var transitionsOutside = currentMapItem.TransitionsOutside;
        var mapItemPosition = new Vector3(currentMapItem.Position.x, currentMapItem.Position.y, 0.0f) + transitionOffset;

        foreach (var transitionInside in transitionsInside)
        {
            var direction = transform.TransformDirection(mapItemPosition - transitionInside.localPosition).normalized;
            transitionInside.forward = direction;

            var zSize = (currentMapItem.Position - transitionInside.localPosition).magnitude * TransitionPrefab.transform.localScale.z;
            transitionInside.localScale = new Vector3(transitionInside.localScale.x, transitionInside.localScale.y, zSize);
        }

        foreach (var transitionOutside in transitionsOutside)
        {
            var endPosition = transitionOutside.Value.localPosition + transitionOffset;

            var direction = transform.TransformDirection(endPosition - mapItemPosition).normalized;
            transitionOutside.Key.forward = direction;

            var zSize = (endPosition - mapItemPosition).magnitude * TransitionPrefab.transform.localScale.z;
            transitionOutside.Key.localScale = new Vector3(transitionOutside.Key.localScale.x, transitionOutside.Key.localScale.y, zSize);

            transitionOutside.Key.localPosition = mapItemPosition;
        }
    }

    private void AddHorizontalTransition(Panorama panorama, ArrowInfo arrow)
    {
        List<TransitionData> arrows;
        Vector3 startPosition;
        Vector3 endPosition;
        MapItem previousMapItem;
        MapItem nextMapItem;

        if (!transitions.TryGetValue(panorama.ImageLink, out arrows))
        {
            arrows = new List<TransitionData>();
            transitions.Add(panorama.ImageLink, arrows);
        }

        if (!panorams.TryGetValue(panorama.ImageLink, out previousMapItem))
        {
            startPosition = Vector3.zero;
            previousMapItem = AddPanoramaItem(panorama.ImageLink, startPosition, CurrentLevel);
        }
        else
        {
            startPosition = previousMapItem.Position;
        }

        float angle = arrow.Rotation.eulerAngles.y;
        if (!panorams.TryGetValue(arrow.NextPanoramaFilename, out nextMapItem))
        {
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f);
            endPosition = startPosition + direction * arrow.Position.z * distanceBetweenPanoramsKoeff;
            nextMapItem = AddPanoramaItem(arrow.NextPanoramaFilename, endPosition, CurrentLevel);
        }
        else
        {
            endPosition = nextMapItem.Position;
        }

        if (AddArrows)
        {
            var transition = new TransitionData();
            transition.NextItem = arrow.NextPanoramaFilename;
            transition.TransitionObject = SpawnTransition(startPosition, endPosition);
            previousMapItem.AddTransitionOutside(transition.TransitionObject.transform, nextMapItem.transform);
            nextMapItem.AddTransitionInside(transition.TransitionObject.transform);


            arrows.Add(transition);
            transitions[panorama.ImageLink] = arrows;
        }
    }

    private void AddVerticalTransition(Panorama panorama, StairsInfo arrow)
    {
        int nextIndex = arrow.StairsIsUp ? CurrentLevel + 1 : CurrentLevel - 1;

        if (levels != null && levels.ContainsKey(CurrentLevel) && panorams.TryGetValue(panorama.ImageLink, out var startItem))
        {
            MapItem mapItem;
            if (!panorams.TryGetValue(arrow.NextPanoramaFilename, out var item))
            {
                mapItem = AddPanoramaItem(arrow.NextPanoramaFilename, startItem.Position, nextIndex);
                mapItem.gameObject.SetActive(false);
            }
            else
            {
                if (item.Level != nextIndex)
                {
                    Debug.LogError("That MapItem is exist on map on other floor.");
                    return;
                }

                mapItem = item;
            }

            mapItem.AddStairs(arrow);
            startItem.AddStairs(arrow);

            if (!levels.ContainsKey(nextIndex))
            {
                levels.Add(nextIndex, new List<MapItem>());

                if (nextIndex < CurrentLevel)
                {
                    MinLevel = nextIndex;
                }
                else
                {
                    MaxLevel = nextIndex;
                }
            }

            if (!levels[nextIndex].Contains(mapItem))
                levels[nextIndex].Add(mapItem);
        }
    }

    private void PanoramaController_TransitionAdded(Panorama panorama, ArrowInfo arrow)
    {
        if (panorams == null) return;

        if (arrow.ArrowType == (int)ArrowType.STAIRS)
        {
            AddVerticalTransition(panorama, (StairsInfo)arrow);
        }
        else
        {
            AddHorizontalTransition(panorama, arrow);

            if (levels != null && panorams.TryGetValue(arrow.NextPanoramaFilename, out var item))
            {
                if (!levels.ContainsKey(CurrentLevel))
                {
                    levels.Add(CurrentLevel, new List<MapItem>());
                }

                if (!levels[CurrentLevel].Contains(item))
                {
                    levels[CurrentLevel].Add(item);
                }
            }
        }
    }

    private MapItem AddPanoramaItem(string panoramaPath, Vector3 position, int level)
    {
        var panorama = Instantiate(MapItem, transform);
        panorama.transform.localPosition = position;

        if (panorama.TryGetComponent<MapItem>(out var component))
        {
            component.PanoramaPath = panoramaPath;
            panorams.Add(panoramaPath, panorama);
            component.Initialize();
            component.Level = level;
            return component;
        }

        return null;
    }

    public void ShowLevel(bool changeCurrentLevel = true)
    {
        ShowLevel(CurrentLevel, changeCurrentLevel);
    }

    public void ShowLevel(int levelNumber, bool changeCurrentLevel = true)
    {
        if (levels == null || !levels.ContainsKey(levelNumber)) return;

        var items = levels[levelNumber];

        foreach (var level in levels)
        {
            if (level.Value != items)
            {
                foreach (var item in level.Value)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    item.gameObject.SetActive(false);

                    if (transitions.TryGetValue(item.PanoramaPath, out var currentTransitions))
                    {
                        foreach (var currentTransition in currentTransitions)
                        {
                            if (currentTransition.TransitionObject != null)
                            {
                                currentTransition.TransitionObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        foreach (var item in items)
        {
            if (!item.gameObject.activeSelf)
                item.gameObject.SetActive(true);

            if (transitions.TryGetValue(item.PanoramaPath, out var currentTransitions))
            {
                foreach (var currentTransition in currentTransitions)
                {
                    if (currentTransition.TransitionObject != null)
                    {
                        currentTransition.TransitionObject.SetActive(true);
                    }
                }
            }
        }

        if (changeCurrentLevel)
        {
            CurrentLevel = levelNumber;
        }

        MapUILevel = levelNumber;
    }

    private GameObject SpawnTransition(Vector3 startPosition, Vector3 endPosition)
    {
        float zSize = (endPosition - startPosition).magnitude * TransitionPrefab.transform.localScale.z;
        var transitionObject = Instantiate(TransitionPrefab, transform);
        transitionObject.transform.forward = transform.TransformDirection((endPosition - startPosition).normalized);
        transitionObject.transform.localPosition = startPosition;
        transitionObject.transform.localScale = new Vector3(transitionObject.transform.localScale.x, transitionObject.transform.localScale.y, zSize);
        transitionObject.transform.localPosition += transitionOffset;

        return transitionObject;
    }

    public void RemoveMap()
    {
        if (panorams != null)
        {
            foreach (var panorama in panorams)
            {
                Destroy(panorama.Value.gameObject);
            }
        }

        if (transitions != null)
        {
            foreach (var transition in transitions)
            {
                foreach (var arrow in transition.Value)
                {
                    Destroy(arrow.TransitionObject);
                }
            }
        }

        panorams = new Dictionary<string, MapItem>();
        transitions = new Dictionary<string, List<TransitionData>>();
        levels = new Dictionary<int, List<MapItem>>();
        CurrentLevel = MapUILevel = 0;
        MaxLevel = MinLevel = 0;
    }

    public IEnumerator UpdateMap(Action<float> checkProgress, Action afterCoroutine, Dictionary<string, Panorama> newPanorams, Dictionary<string, Vector3> mapItemsPositions = null, Dictionary<string, int> mapLevels = null)
    {
        RemoveMap();

        float allArrowsCount = 0;
        foreach(var newPanorama in newPanorams)
        {
            allArrowsCount += newPanorama.Value.ArrowsInfo.Count;
        }

        float currentArrowIndex = 0;

        foreach (var newPanorama in newPanorams)
        {
            var arrows = newPanorama.Value.ArrowsInfo;

            if (mapLevels != null)
            {
                if (mapLevels.TryGetValue(newPanorama.Value.ImageLink, out var level))
                {
                    CurrentLevel = level;
                }
            }

            foreach (var arrow in arrows)
            {
                PanoramaController_TransitionAdded(newPanorama.Value, arrow);
                currentArrowIndex++;
                checkProgress?.Invoke((currentArrowIndex / allArrowsCount) * 100);
                yield return new WaitForEndOfFrame();
            }
        }

        if (mapItemsPositions != null)
        {
            foreach (var panorama in panorams)
            {
                if (mapItemsPositions.TryGetValue(panorama.Value.PanoramaPath, out var newPosition) &&
                    panorams.TryGetValue(panorama.Value.PanoramaPath, out var mapItem))
                {
                    mapItem.Position = newPosition;
                    UpdateMapItem(mapItem);
                }
            }
        }

        ShowLevel(MaxLevel);

        if (UIController.Instance != null)
        {
            UIController.Instance.CheckMapUI();
        }

        afterCoroutine?.Invoke();
    }

    public void UpdateMap(Dictionary<string, Panorama> newPanorams, Dictionary<string, Vector3> mapItemsPositions = null, Dictionary<string, int> mapLevels = null)
    {
        if (newPanorams == null) return;

        RemoveMap();

        foreach(var newPanorama in newPanorams)
        {
            var arrows = newPanorama.Value.ArrowsInfo;

            if (mapLevels != null)
            {
                if (mapLevels.TryGetValue(newPanorama.Value.ImageLink, out var level))
                {
                    CurrentLevel = level;
                }
            }

            foreach (var arrow in arrows)
            {

                PanoramaController_TransitionAdded(newPanorama.Value, arrow);
            }
        }

        if (mapItemsPositions != null)
        {
            foreach (var panorama in panorams)
            {
                if (mapItemsPositions.TryGetValue(panorama.Value.PanoramaPath, out var newPosition) &&
                    panorams.TryGetValue(panorama.Value.PanoramaPath, out var mapItem))
                {
                    mapItem.Position = newPosition;
                    UpdateMapItem(mapItem);
                }
            }
        }

        ShowLevel(MaxLevel);

        if(UIController.Instance != null)
        {
            UIController.Instance.CheckMapUI();
        }
    }


    private void MoveMap()
    {
        if (input != null)
        {
            if (input.GetFirstKey())
            {
                var diffCursorPosition = input.DiffCursorPosition();
                var diffVector = new Vector3(diffCursorPosition.x + (Screen.width * 0.5f), diffCursorPosition.y + (Screen.height * 0.5f), transform.localPosition.z);
                var diffPosition = Camera.main.transform.InverseTransformVector(Camera.main.ScreenToWorldPoint(diffVector));

                transform.localPosition = startPosition + new Vector3(diffPosition.x, diffPosition.y, 0);
            }
            else
            {
                startPosition = transform.localPosition;
            }

            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, minPoint.x, maxPoint.x                         ),
                                                  Mathf.Clamp(transform.localPosition.y, minPoint.y, maxPoint.y                         ), 
                                                  Mathf.Clamp(transform.localPosition.z - input.ScrollDelta(), minDistance, maxDistance));
        }
    }

    private void Update()
    {
        if (allowToMove)
            MoveMap();
    }

    public List<MapItemData> GetMapItemsData()
    {
        if (panorams != null)
        {
            var data = new List<MapItemData>();

            foreach (var panorama in panorams)
            {
                var itemData = new MapItemData()
                {
                    PanoramaPath = panorama.Value.PanoramaPath,
                    MapItemPosition = panorama.Value.Position,
                    Level = panorama.Value.Level
                };

                data.Add(itemData);
            }

            return data;
        }

        return null;
    }

    public List<LevelInfo> GetLevelsInfo()
    {
        var result = new List<LevelInfo>();

        foreach(var level in levels)
        {
            var levelInfo = new LevelInfo();
            levelInfo.Index = level.Key;

            var panoramaFilenames = new List<string>();
            foreach(var name in level.Value)
            {
                panoramaFilenames.Add(name.PanoramaPath);
            }

            levelInfo.PanoramaFilenames = panoramaFilenames;

            result.Add(levelInfo);
        }

        return result;
    }

    public Dictionary<string, Vector3> MapPositionsToDictionary(List<MapItemData> mapData)
    {
        if(mapData != null && mapData.Count > 0)
        {
            var dictionary = new Dictionary<string, Vector3>();

            foreach(var item in mapData)
            {
                dictionary.Add(item.PanoramaPath, item.MapItemPosition);
            }

            return dictionary;
        }

        return null;
    }

    public Dictionary<string, int> MapLevelsToDictionary(List<MapItemData> mapData)
    {
        if (mapData != null && mapData.Count > 0)
        {
            var dictionary = new Dictionary<string, int>();

            foreach (var item in mapData)
            {
                dictionary.Add(item.PanoramaPath, item.Level);
            }

            return dictionary;
        }

        return null;
    }

    private struct TransitionData
    {
        public string NextItem;
        public GameObject TransitionObject;
    }
}
