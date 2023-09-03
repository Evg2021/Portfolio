using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TrenObjectInitializator : SingletonMonoBehaviour<TrenObjectInitializator>
{
    private static string[] restoreMessages = { "Восстановление работоспособности", "Исправен" };

    private const string bindingsFilename = "Bindings.json";
    private const string pipesDataPointer = "#####";
    private const string pipesDataPattern = @"\[.*?\]";
    private const char rowsSpliter = ';';

    public List<PipeData> PipesData;
    public TrenObject[] TrenObjects;

    private List<TrenObjectData> trenObjectsBindings;

    private DefectsList defectsList;
    private static readonly Queue<Action> defectsQueue = new Queue<Action>();
    private static Dictionary<string, GameObject> defectInstances;
    public static Dictionary<string, GameObject> DefectInstances => defectInstances;

    // Start is called before the first frame update
    void Start()
    {
        var ipadress = string.IsNullOrEmpty(Settings.IPAddress) ? GlobalVariables.LocalIPAdress : Settings.IPAddress;
        var message = ClientSocketManager.CreateConnect(ipadress, 20200, OnDefect);
        if (message == 0)
        {
            if (InitializeBindings())
            {
                InitializeTrenObjects();
                InitializeInteractableObjects();
                InitializePipesMap();
                InitializeDefectsList();
                InitializeDefectInstances();
            }
        }
        else
        {
            Debug.LogError(message);
        }

    }

    private void Update()
    {
        lock (defectsQueue)
        {
            while (defectsQueue.Count > 0)
            {
                defectsQueue.Dequeue()?.Invoke();
            }
        }
    }

    private static void OnDefect(IntPtr objectName, IntPtr defectName)
    {
        try
        {
            //string encodedObjectName = Utilities.GetStringFromIntPtr(objectName, Encoding.GetEncoding("windows-1251"));
            //string encodedDefectName = Utilities.GetStringFromIntPtr(defectName, Encoding.GetEncoding("windows-1251"));

            byte[] name = Utilities.GetBytesFromIntPtr(objectName);
            byte[] defect = Utilities.GetBytesFromIntPtr(defectName);

            lock (defectsQueue)
            {
                defectsQueue.Enqueue(() => Instance.StartCoroutine(Instance.DefineDefect(name, defect)));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    private IEnumerator DefineDefect(byte[] bytesName, byte[] bytesDefect)
    {
        string objectName = Encoding.GetEncoding("windows-1251").GetString(bytesName);
        string defectName = Encoding.GetEncoding("windows-1251").GetString(bytesDefect);

        Debug.Log($"ObjectName: {objectName}");
        Debug.Log($"DefectName: {defectName}");

        if (TrenObjects != null && TrenObjects.Length > 0 && defectsList)
        {
            var defectsItems = defectsList.DefectsItems;
            if (defectsItems != null && defectsItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(objectName) && !string.IsNullOrEmpty(defectName))
                {
                    var trenObject = TrenObjects.FirstOrDefault(h => h.TrenName == objectName);
                    if (trenObject)
                    {
                        if (restoreMessages.Any(defectName.Contains))
                        {
                            RemoveDefect(trenObject.name);
                        }
                        else
                        {
                            var defect = defectsItems.FirstOrDefault(h => h.DefectName == defectName);
                            Debug.Log($"Defect definded: {trenObject != null && defect.Effect != null}");
                            if (defect.Effect && !defectInstances.ContainsKey(trenObject.name))
                            {
                                defectInstances.Add(trenObject.name, Instantiate(defect.Effect, trenObject.transform));
                            }
                        }
                    }
                }
            }
        }

        yield return null;
    }
    public static void RemoveDefect(string key)
    {
        if (defectInstances.TryGetValue(key, out var defect))
        {
            if (defect && defect.TryGetComponent<ParticleSystem>(out var effect))
            {
                effect.Stop();
                Debug.Log($"Defect removed from {key}.");
            }

            defectInstances.Remove(key);
        }
    }

    private bool InitializeBindings()
    { 
        bool value = false;

        trenObjectsBindings = new List<TrenObjectData>();

        if (Utilities.CheckStreamingAssetsPath())
        {
            string pathToBindings = Application.streamingAssetsPath + '\\' + bindingsFilename;

//#if UNITY_EDITOR
//            string bindingsName = "StreamingAssets\\HS\\Bindings_170.json";
//            pathToBindings = Directory.GetCurrentDirectory() + "\\Assets\\" + bindingsName;
//#endif

            if (File.Exists(pathToBindings))
            {
                using (StreamReader reader = new StreamReader(pathToBindings))
                {
                    try
                    {
                        var data = JsonUtility.FromJson<TrenObjectDataList>(reader.ReadToEnd());
                        trenObjectsBindings = data.Data;
                        if (trenObjectsBindings != null && trenObjectsBindings.Count > 0)
                        {
                            value = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }

        return value;
    }

    private void InitializeInteractableObjects()
    {
        var interactables = FindObjectsOfType<InteractableObject>();
        foreach (var interactable in interactables)
        {
            interactable.Initialize();
        }
    }

    private void InitializeTrenObjects()
    {
        if (trenObjectsBindings != null || trenObjectsBindings.Count > 0)
        {
            var trenObjectsDictionary = new Dictionary<uint, TrenObject>();

            foreach (var data in trenObjectsBindings)
            {
                var unityObject = GameObject.Find(data.UnityName);
                if (unityObject && unityObject.TryGetComponent<ControllerBase>(out var controller))
                {
                    TrenObject newTrenObject = null;

                    if (controller.GetTypesCount() == 1)
                    {
                        if (controller.GetControllerType() == Types.TYPE_FLOAT)
                        {
                            newTrenObject = unityObject.AddComponent<TrenFloat>();
                        }
                        else if (controller.GetControllerType() == Types.TYPE_BOOL)
                        {
                            newTrenObject = unityObject.AddComponent<TrenBool>();
                        }
                        else if (controller.GetControllerType() == Types.TYPE_INT)
                        {
                            newTrenObject = unityObject.AddComponent<TrenInt>();
                        }
                    }
                    else if (controller.GetTypesCount() >= 2)
                    {
                        var multiTypesController = controller as IMultiTypesController;
                        if (multiTypesController != null)
                        {
                            if (data.ObjectMode == ObjectMode.SET)
                            {
                                if (multiTypesController.GetControllerTypeSet() == Types.TYPE_FLOAT)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenFloat>();
                                }
                                else if (multiTypesController.GetControllerTypeSet() == Types.TYPE_BOOL)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenBool>();
                                }
                                else if (multiTypesController.GetControllerTypeSet() == Types.TYPE_INT)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenInt>();
                                }
                            }
                            else if (data.ObjectMode == ObjectMode.GET)
                            {
                                if (multiTypesController.GetControllerTypeGet() == Types.TYPE_FLOAT)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenFloat>();
                                }
                                else if (multiTypesController.GetControllerTypeGet() == Types.TYPE_BOOL)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenBool>();
                                }
                                else if (multiTypesController.GetControllerTypeGet() == Types.TYPE_INT)
                                {
                                    newTrenObject = unityObject.AddComponent<TrenInt>();
                                }
                            }
                        }
                    }

                    if (newTrenObject != null)
                    {
                        newTrenObject.Initialize(data.TrenName, data.TrenParameter, data.ObjectMode);
                        newTrenObject.RegistrateObject();

                        if (newTrenObject.Index != uint.MaxValue)
                        {
                            trenObjectsDictionary.Add(newTrenObject.Index, newTrenObject);
                        }
                    }
                }
                else if(!unityObject)
                {
                    var splittedName = data.UnityName.Split('\'');
                    if (splittedName.Length > 1)
                    {
                        unityObject = GameObject.Find(splittedName[0]);
                        if (unityObject && unityObject.TryGetComponent<InteractableObjectWithPult>(out var interactableWithPult))
                        {
                            interactableWithPult.AddBinding(data);
                        }
                    }
                }
            }

            if (trenObjectsDictionary.Count > 0)
            {
                TrenObjects = new TrenObject[trenObjectsDictionary.Count];
                foreach (var trenObject in trenObjectsDictionary)
                {
                    TrenObjects[trenObject.Key] = trenObject.Value;
                }
            }
        }
    }
    private void InitializePipesMap()
    {
        CollectPipes(Directory.GetCurrentDirectory() + "\\visio\\", TrenObjects);
    }
    private void InitializeDefectsList()
    {
        defectsList = DefectsList.Instance;

        if (!defectsList)
        {
            Debug.LogError("Defects List Instance is not found.");
        }
    }
    private void InitializeDefectInstances()
    {
        defectInstances = new Dictionary<string, GameObject>();
    }

    private void CollectPipes(string path, TrenObject[] trenObjects)
    {
        if (Directory.Exists(path))
        {
            string[] fileNames = Directory.GetFiles(path, "*.csv");
            if (fileNames != null)
            {
                PipesData = new List<PipeData>();
                foreach (var fileName in fileNames)
                {
                    var rows = File.ReadAllLines(fileName, Encoding.GetEncoding("windows-1251"));
                    bool isPipesData = false;
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (isPipesData)
                        {
                            var rowsData = Regex.Matches(rows[i], pipesDataPattern);
                            var pipeData = new PipeData();
                            var inObjects = new List<Transform>();
                            var outObjects = new List<Transform>();

                            for (int j = 0; j < rowsData.Count; j++)
                            {
                                string clearedData = rowsData[j].Value.Replace("[", string.Empty)
                                                                      .Replace("]", string.Empty)
                                                                      .Replace("\"", string.Empty);

                                if (j == 0)
                                {
                                    pipeData.PipeName = clearedData;
                                }
                                else if (j % 2 == 1)
                                {
                                    var trenObjectOnScene = trenObjects.FirstOrDefault(h => h.TrenName == clearedData);

                                    if (j < rowsData.Count - 1 && trenObjectOnScene)
                                    {
                                        string enterType = rowsData[j + 1].Value.Replace("[", string.Empty)
                                                                              .Replace("]", string.Empty)
                                                                              .Replace("\"", string.Empty);
                                        if (enterType.Contains('o'))
                                        {
                                            outObjects.Add(trenObjectOnScene.transform);
                                        }
                                        else
                                        {
                                            inObjects.Add(trenObjectOnScene.transform);
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(pipeData.PipeName) && (outObjects.Count > 0 || inObjects.Count > 0))
                            {
                                pipeData.inTrenObjects = inObjects.ToArray();
                                pipeData.outTrenObject = outObjects.ToArray();
                                PipesData.Add(pipeData);
                            }
                        }

                        if (rows[i].Contains(pipesDataPointer))
                        {
                            isPipesData = true;
                        }
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        ClientSocketManager.ClearRegistration();
        ClientSocketManager.CloseConnect();
    }
}
