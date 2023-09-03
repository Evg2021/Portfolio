using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ControllersAssigner : Editor
{
    private const string pathToFunctions = "T-Soft/Utilities/";
    private const string Pult18Path = "Assets\\Prefabs\\UI\\Pults\\Pult18.prefab";

    private static string[] valveInteractableNames = { "valve" , "klapan" };
    private static string[] showingInteractableNames = { "sensor" };
    private static string[] interactableNames = { "damper" };
    private static string[] mainManipulatorKeyNames = { "Manipulator", ManometrKeyName, "handle", "button" };
    private static string[] interactablesWithPultKeynames = { "Flap18" };

    private const string ValveManipulatorKeyName = "ValveFWManipulator";
    private const string RodManipulatorKeyName = "ValveRodManipulator";
    private const string HandleManipulatorKeyName = "HandleManipulator";
    private const string EightManipulatorKeyName = "EightManipulator";
    private const string FloatHManipulator = "FloatHManipulator";
    private const string ManometrKeyName = "Manometr";
    private const string StartStopHandleKeyName = "startstophandle";
    private const string SwingKeyName = "Swing";
    private const string SwitchHKeyName = "SWITCHH";
    private const string PendulumKeyName = "Pendulum";
    //if flap has no valve.
    private const string FlapInsideOfalveKeyName = "Flap";

    [MenuItem(pathToFunctions + "Setup controllers in selected object")]
    public static void SetupControllersInSelectedObject()
    {
        var currentObject = Selection.activeGameObject;
        if (currentObject)
        {
            var interactables = currentObject.GetComponentsInChildren<Transform>()
                                             .Where(h => valveInteractableNames.Any(h.name.Contains) || 
                                                         showingInteractableNames.Any(h.name.Contains) || 
                                                         interactableNames.Any(h.name.Contains));
            if (interactables != null && interactables.Count() > 0)
            {
                int interactablesCount = 0;
                int manipulatorsCount = 0;

                foreach (var interactable in interactables)
                {
                    var manipulators = interactable.GetComponentsInChildren<Transform>().Where(h => mainManipulatorKeyNames.Any(h.name.Contains));

                    if (manipulators != null && manipulators.Count() > 0)
                    {
                        foreach (var manipulator in manipulators)
                        {
                            if (DefineManipulator(manipulator))
                            {
                                manipulatorsCount++;
                            }
                        }
                    }

                    if (!interactable.TryGetComponent<InteractableObject>(out var interactableComponent))
                    {
                        interactableComponent = interactable.gameObject.AddComponent<InteractableObject>();
                        interactablesCount++;
                    }

                    if (interactableComponent.mainController == null)
                    {
                        if (valveInteractableNames.Any(interactable.name.Contains))
                        {
                            interactableComponent.mainController = interactable.GetComponentInChildren<ValveController>();
                            if (!interactableComponent.mainController)
                            {
                                var flap = interactableComponent.GetComponentsInChildren<Transform>().FirstOrDefault(h => h.name.Contains(FlapInsideOfalveKeyName));
                                if (flap)
                                {
                                    if (!flap.TryGetComponent<ValuesShowingController>(out var valuesShowingController))
                                    {
                                        valuesShowingController = flap.gameObject.AddComponent<ValuesShowingController>();
                                    }
                                    else
                                    {
                                        interactableComponent.mainController = valuesShowingController;
                                    }
                                }                                
                            }
                        }

                        if (showingInteractableNames.Any(interactable.name.Contains))
                        {
                            interactableComponent.mainController = interactable.GetComponentInChildren<ValuesShowingController>();
                        }
                    }

                    if (interactableComponent.secondController == null)
                    {
                        if (valveInteractableNames.Any(interactable.name.Contains) || interactableNames.Any(interactable.name.Contains))
                        {
                            var handle = interactable.GetComponentInChildren<HandleBoolController>();
                            if (handle)
                            {
                                interactableComponent.secondController = handle;
                            }
                        }
                    }
                }

                Debug.Log($"Setup controllers: {manipulatorsCount}");
                Debug.Log($"Setup InteractableObjects: {interactablesCount}");
            }
        }
        else
        {
            Debug.LogError("Select parent with objects to installing controllers.");
        }
    }

    [MenuItem(pathToFunctions + "Setup controllers in selected Interactables")]
    public static void SetupControllersInInteractables()
    {
        int interactablesCount = 0;
        int manipulatorsCount = 0;

        var selected = Selection.gameObjects;

        for (int i = 0; i < selected.Length; i++)
        {
            InteractableObject currentInteractable = null;
            if (!selected[i].TryGetComponent(out currentInteractable))
            {
                currentInteractable = selected[i].AddComponent<InteractableObject>();
                interactablesCount++;
            }

            var manipulators = currentInteractable.GetComponentsInChildren<Transform>().Where(h => mainManipulatorKeyNames.Any(h.name.Contains));

            if (manipulators != null && manipulators.Count() > 0)
            {
                foreach (var manipulator in manipulators)
                {
                    if (DefineManipulator(manipulator))
                    {
                        manipulatorsCount++;
                    }
                }
            }

            if (currentInteractable.mainController == null)
            {
                if (valveInteractableNames.Any(currentInteractable.name.Contains))
                {
                    currentInteractable.mainController = currentInteractable.GetComponentInChildren<ValveController>();
                    if (!currentInteractable.mainController)
                    {
                        var flap = currentInteractable.GetComponentsInChildren<Transform>().FirstOrDefault(h => h.name.Contains(FlapInsideOfalveKeyName));
                        if (flap)
                        {
                            if (!flap.TryGetComponent<ValuesShowingController>(out var valuesShowingController))
                            {
                                valuesShowingController = flap.gameObject.AddComponent<ValuesShowingController>();
                            }
                            else
                            {
                                currentInteractable.mainController = valuesShowingController;
                            }
                        }
                    }
                }
            }

            if (currentInteractable.secondController == null)
            {
                currentInteractable.secondController = currentInteractable.GetComponentInChildren<HandleBoolController>();
            }
        }

        Debug.Log($"Setup controllers: {manipulatorsCount}");
        Debug.Log($"Setup InteractableObjects: {interactablesCount}");
    }

    [MenuItem(pathToFunctions + "Setup LODs to Valves")]
    public static void SetupLODsToValves()
    {
        SetupLODsToObjects(ValveManipulatorKeyName);
    }
    
    [MenuItem(pathToFunctions + "Setup LODs to Flaps")]
    public static void SetupLODsToFlaps()
    {
        SetupLODsToObjects(FlapInsideOfalveKeyName);
    }

    [MenuItem(pathToFunctions + "Setup LODs to Bases")]
    public static void SetupLODsToBases()
    {
        SetupLODsToObjects("Base");
    }

    [MenuItem(pathToFunctions + "Setup LODs to OZL")]
    public static void SetupLODsToOZLs()
    {
        int count = 0;
        var ozls = FindObjectsOfType<Transform>().Where(h => h.name.Contains("OZL"));
        
        foreach (var ozl in ozls)
        {
            var arguments = ozl.name.Split('_');
            float lod = 0.02f;
            if (arguments != null && arguments.Length > 0)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] == "OZL" && i < arguments.Length - 1)
                    {
                        if (float.TryParse(arguments[i + 1], out float argument))
                        {
                            lod = argument * 0.01f;
                        }
                    }
                }

                SetupLODToObject(ozl, lod);
                count++;
            }
        }

        Debug.Log("OZL objects get LOD: " + count);
    }

    [MenuItem(pathToFunctions + "Setup Colliders to Meshes in Selected Object")]
    public static void SetupCollidersToMeshesInSelectedObjet()
    {
        var selectedGameObjects = Selection.gameObjects;
        if (selectedGameObjects != null && selectedGameObjects.Length > 0)
        {
            int countAdd = 0;
            int countChange = 0;

            for (int i = 0; i < selectedGameObjects.Length; i++)
            {
                var currentObject = selectedGameObjects[i];
                var interactables = currentObject.GetComponentsInChildren<Transform>().Where(h => valveInteractableNames.Any(h.name.Contains) ||                                                                                                  interactableNames.Any(h.name.Contains) ||
                                                                                                  showingInteractableNames.Any(h.name.Contains) ||
                                                                                                  mainManipulatorKeyNames.Any(h.name.Contains));
                foreach (var interactable in interactables)
                {
                    var renderers = interactable.GetComponentsInChildren<MeshRenderer>();
                    foreach (var renderer in renderers)
                    {
                        if (!renderer.TryGetComponent<MeshCollider>(out var collider))
                        {
                            collider = renderer.gameObject.AddComponent<MeshCollider>();
                            countAdd++;
                        }

                        if (!collider.convex)
                        {
                            countChange++;
                            collider.convex = true;
                        }
                    }
                }
            }

            Debug.Log("Colliders added: " + countAdd);
            Debug.Log("Colliders changed: " + countChange);
        }
        else
        {
            Debug.LogError("Select objects to installing colliders.");
        }
    }

    [MenuItem(pathToFunctions + "Setup Doors")]
    public static void SetupDoorsControllersToDoorsObjects()
    {
        int collidersCount = 0;
        int doorsCount = 0;
        var doorsObjects = FindObjectsOfType<Transform>().Where(h => h.name.Contains("interactDoor")).ToArray();

        for (int i = 0; i < doorsObjects.Length; i++)
        {
            if (!doorsObjects[i].TryGetComponent<MeshCollider>(out var collider))
            {
                doorsObjects[i].gameObject.AddComponent<MeshCollider>();
                collidersCount++;
            }
            else
            {
                if (!collider.enabled)
                {
                    collider.enabled = true;
                }
            }

            if (!doorsObjects[i].TryGetComponent<DoorController>(out var controller))
            {
                controller = doorsObjects[i].gameObject.AddComponent<DoorController>();
                doorsCount++;
            }

            string splitted = controller.name.Split('_')[0];
            if (splitted[splitted.Length - 1] == 'L')
            {
                controller.AxisRotation = Vector3.forward;
            }
            else if (splitted[splitted.Length - 1] == 'R')
            {
                controller.AxisRotation = -Vector3.forward;
            }
        }

        Debug.Log($"Installed colliders on doors: {collidersCount}.");
        Debug.Log($"Installed Doors: {doorsCount}.");
    }

    [MenuItem(pathToFunctions + "Setup all klapans with flap18")]
    public static void SetupInteractablesWithPult18()
    {
        int addedInteractablesWithPultCount = 0;

        var klapans18 = FindObjectsOfType<Transform>().Where(h => interactablesWithPultKeynames.Any(h.name.Contains)).ToArray();

        for (int i = 0; i < klapans18.Length; i++)
        {
            var klapanParent = klapans18[i].parent?.parent;
            if (klapanParent)
            {
                ControllerBase mainController = null;
                ControllerBase secondController = null;

                if (!klapanParent.TryGetComponent<InteractableObjectWithPult>(out var interactableWithPult))
                {
                    addedInteractablesWithPultCount++;

                    
                    if (klapanParent.TryGetComponent<InteractableObject>(out var interactable))
                    {
                        mainController = interactable.mainController;
                        secondController = interactable.secondController;
                        DestroyImmediate(interactable);
                    }
                    else
                    {
                        mainController = klapanParent.GetComponentInChildren<ValveController>();
                        secondController = klapanParent.GetComponentInChildren<HandleBoolController>();
                    }

                    interactableWithPult = klapanParent.gameObject.AddComponent<InteractableObjectWithPult>();
                }
                else
                {
                    var interactables = klapanParent.GetComponents<InteractableObject>();
                    for (int j = 0; j < interactables.Length; j++)
                    {
                        if (interactables[j].GetType() != typeof(InteractableObjectWithPult))
                        {
                            DestroyImmediate(interactables[j]);
                        }
                    }

                    mainController = klapanParent.GetComponentInChildren<ValveController>();
                    secondController = klapanParent.GetComponentInChildren<HandleBoolController>();
                }

                interactableWithPult.mainController = mainController;
                interactableWithPult.secondController = secondController;
                
                string path = Directory.GetCurrentDirectory() + Pult18Path;
                interactableWithPult.PultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Pult18Path);
            }
        }

        Debug.Log($"Added InteractableObjects with Pult: {addedInteractablesWithPultCount}.");
    }

    private static bool DefineManipulator(Transform manipulator)
    {
        bool definded = false;

        if (manipulator.name.Contains(ValveManipulatorKeyName))
        {
            definded = AssignController<ValveController>(manipulator);
        }
        else if (manipulator.name.Contains(FloatHManipulator))
        {
            if (manipulator.parent && manipulator.parent.parent && valveInteractableNames.Any(manipulator.parent.parent.name.Contains))
            {
                definded = AssignController<ValveController>(manipulator);
            }
        }
        else if (manipulator.name.Contains(RodManipulatorKeyName))
        {
            definded = AssignController<RodController>(manipulator);
        }
        else if (manipulator.name.Contains(HandleManipulatorKeyName))
        {
            if (manipulator.TryGetComponent<MeshRenderer>(out _))
            {
                definded = AssignController<CurveHandleBoolController>(manipulator);
            }
            else
            {
                definded = AssignController<HandleBoolController>(manipulator);
            }
        }
        else if (manipulator.name.Contains(ManometrKeyName))
        {
            definded = AssignController<ValuesShowingController>(manipulator);
        }
        else if (manipulator.name.Contains(EightManipulatorKeyName))
        {
            definded = AssignController<DamperBoolController>(manipulator);
        }
        else if (manipulator.name.Contains(StartStopHandleKeyName))
        {
            if (manipulator.parent)
            {
                if (manipulator.parent.name.Contains(SwingKeyName))
                {
                    definded = AssignController<SwingerBoolController>(manipulator);
                }
                else if (manipulator.parent.name.Contains(SwitchHKeyName) ||
                         manipulator.parent.name.Contains(PendulumKeyName))
                {
                    definded = AssignController<SwitchHBoolController>(manipulator);
                }
            }
        }

        return definded;
    }

    private static bool AssignController<T>(Transform manipulator)
        where T : MonoBehaviour
    {
        if (!manipulator.TryGetComponent<T>(out _))
        {
            manipulator.gameObject.AddComponent<T>();
            return true;
        }

        return false;
    }
    
    private static void SetupLODsToObjects(string keyname)
    {
        var allObjects = FindObjectsOfType<Transform>().Where(h => h.name.Contains(keyname));

        int addedCount = 0;
        int customizedCount = 0;

        foreach (var obj in allObjects)
        {
            LODGroup group;
            if (!obj.TryGetComponent(out group))
            {
                group = obj.gameObject.AddComponent<LODGroup>();
                addedCount++;
            }

            var lods = new LOD[1];
            lods[0] = new LOD(0.02f, obj.GetComponents<Renderer>());
            group.SetLODs(lods);
            customizedCount++;
        }

        Debug.Log($"Added LODs: {addedCount}");
        Debug.Log($"Customized LODs: {customizedCount}");
    }
    private static void SetupLODToObject(Transform obj, float value = 0.02f)
    {
        LODGroup group;
        if (!obj.TryGetComponent(out group))
        {
            group = obj.gameObject.AddComponent<LODGroup>();
        }

        var lods = new LOD[1];
        lods[0] = new LOD(value, obj.GetComponents<Renderer>());
        group.SetLODs(lods);
    }
}
