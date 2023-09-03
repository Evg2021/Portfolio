using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScenarioStage : MonoBehaviour
{
    public bool IsTransitionalStage = false;

    [Header("Target Key Object for Camera:")]
    public KeyObject KeyObject;

    [Space(10)]

    [Header("Actions on Stage:")]
    public UnityEvent BeforeStartStage;
    public UnityEvent OnStartStage;
    public UnityEvent BeforeEndStage;
    public UnityEvent OnEndStage;

    [Space(10)]

    [Header("Description Windows:")]
    [TextArea]
    public string MainWindowMessage;

    [Space(10)]

    public List<DescriptionController> DescriptionWindows;
    public List<GradientController> GradientObjects;

    public void SetTransitionalStatus(bool isTransitionalStage, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
    {
        IsTransitionalStage = isTransitionalStage;
        if (isTransitionalStage)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
    public void SetDescriptionWindows(List<DescriptionWindowInfo> descriptionsInfo)
    {
        DescriptionWindows = new List<DescriptionController>();
        var canvas = Utility.GetCanvas3D();
        var prefab = Utility.GetDescriptionWindow3DPrefab();

        if (canvas != null && prefab != null)
        {
            foreach (var info in descriptionsInfo)
            {
                var descriptionObject = Instantiate(prefab, info.Position, info.Rotation, canvas.transform);
                descriptionObject.name = info.Name;
                if (descriptionObject.TryGetComponent<DescriptionController>(out var controller))
                {
                    float[,] args = null;
                    bool[] loopParams = null;
                    if (info.Arguments != null && info.Arguments.Length > 0)
                    { 
                        args = new float[info.Arguments.Length, 3];
                        for (int i = 0; i < info.Arguments.Length; i++)
                        {
                            args[i, 0] = info.Arguments[i];
                            args[i, 1] = info.StepArguments[i];
                            args[i, 2] = info.MaxValues[i];
                        }
                    }
                    if (info.LoopParams != null)
                    {
                        loopParams = new bool[info.LoopParams.Length];
                        for (int i = 0; i < info.LoopParams.Length; i++)
                        {
                            loopParams[i] = info.LoopParams[i];
                        }
                    }

                    controller.Initialize(info.Message, info.Title, args, loopParams);
                    DescriptionWindows.Add(controller);
                }
                else
                {
                    Debug.LogError("Prefab + \"" + prefab.name + "\" has no DescriptionController component.");
                    return;
                }
            }
        }
    }
    public void SetGradientObjects(List<GradientObjectInfo> gradientsInfo)
    {
        GradientObjects = new List<GradientController>();
        var canvas = Utility.GetCanvas3D();
        var prefab = Utility.GetGradientObjectPrefab();

        if (canvas != null && prefab != null)
        {
            foreach (var info in gradientsInfo)
            {
                var prefabObject = Instantiate(prefab, info.Position, info.Rotation, canvas.transform);
                prefabObject.name = info.Name;
                if (prefabObject.TryGetComponent<GradientController>(out var controller))
                {
                    controller.Initialize(info.IsIncreasing);
                    GradientObjects.Add(controller);
                }
                else
                {
                    Debug.LogError("Prefab + \"" + prefab.name + "\" has no GradientController component.");
                    return;
                }
            }
        }
    }
    public bool SetKeyObject(KeyObjectStruct keyObjectInfo)
    {
        var keyObject = GameObject.Find(keyObjectInfo.Name);
        if(keyObject != null)
        {
            if(keyObject.TryGetComponent<KeyObject>(out var keyObjectComponent))
            {
                KeyObject = keyObjectComponent;
                keyObjectComponent.CreateViewPoint(keyObjectInfo.Track.ViewPosition, keyObjectInfo.Track.ViewRotation, ScenarioCreator.GetViewPositionsHolder().transform);
                keyObjectComponent.SetDescriptionInfo(keyObjectInfo.Description);
                return true;
            }
            else
            {
                var keyObjectChild = keyObject.transform.Find(keyObjectInfo.Name);
                if(keyObjectChild != null)
                {
                    if (keyObjectChild.TryGetComponent<KeyObject>(out var keyObjectChildComponent))
                    {
                        KeyObject = keyObjectChildComponent;
                        keyObjectChildComponent.CreateViewPoint(keyObjectInfo.Track.ViewPosition, keyObjectInfo.Track.ViewRotation, ScenarioCreator.GetViewPositionsHolder().transform);
                        keyObjectChildComponent.SetDescriptionInfo(keyObjectInfo.Description);
                        return true;
                    }
                }
            }
        }
        else
        {
            keyObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(h => h.name == keyObjectInfo.Name);
            if (keyObject != null && keyObject.name == keyObjectInfo.Name)
            {
                if (keyObject.TryGetComponent<KeyObject>(out var keyObjectComponent))
                {
                    KeyObject = keyObjectComponent;
                    keyObjectComponent.CreateViewPoint(keyObjectInfo.Track.ViewPosition, keyObjectInfo.Track.ViewRotation, ScenarioCreator.GetViewPositionsHolder().transform);
                    keyObjectComponent.SetDescriptionInfo(keyObjectInfo.Description);
                    return true;
                }
            }
        }

        return false;
    }
    public void SetBeforeStartStage(List<StageAction> actions)
    {
        SetStage(out BeforeStartStage, actions);
    }
    public void SetStartStage(List<StageAction> actions)
    {
        SetStage(out OnStartStage, actions);
    }
    public void SetBeforeEndStage(List<StageAction> actions)
    {
        SetStage(out BeforeEndStage, actions);
    }
    public void SetEndStage(List<StageAction> actions)
    {
        SetStage(out OnEndStage, actions);
    }
    public void SetStage(out UnityEvent unityEvent, List<StageAction> actions)
    {
        unityEvent = new UnityEvent();

        foreach(var action in actions)
        {
            var target = GameObject.Find(action.ObjectName);
            if(target != null)
            {
                if (target.TryGetComponent(Type.GetType(action.TypeName), out var targetComponent))
                {
                    Utility.AddVoidPersistentListener(unityEvent, targetComponent, action.ActionName);
                }
            }
            else
            {
                var objectTarget = Resources.FindObjectsOfTypeAll(Type.GetType(action.TypeName)).FirstOrDefault(h => h.name == action.ObjectName);
                if (objectTarget != null)
                {
                    Utility.AddVoidPersistentListener(unityEvent, objectTarget, action.ActionName);
                }
            }
        }
    }
    public void SetMainMessage(string message)
    {
        MainWindowMessage = message;
    }
    public ScenarioStage GetPrevousStage()
    {
        if (transform.parent != null)
        {
            for (int i = 1; i < transform.parent.childCount; i++)
            {
                var neededIndex = transform.GetSiblingIndex() - i;
                if (neededIndex < 0)
                {
                    neededIndex = transform.parent.childCount + neededIndex;
                }

                var stage = transform.parent.GetChild(neededIndex).GetComponent<ScenarioStage>();
                if (!stage.IsTransitionalStage)
                {
                    return transform.parent.GetChild(neededIndex).GetComponent<ScenarioStage>();
                }
            }
        }

        return null;
    }
    public ScenarioStage GetNextStage()
    {
        if (transform.parent != null)
        {
            for (int i = 1; i < transform.parent.childCount; i++)
            {
                var neededIndex = transform.GetSiblingIndex() + i;
                if (neededIndex >= transform.parent.childCount)
                {
                    neededIndex %= transform.parent.childCount;
                }

                var stage = transform.parent.GetChild(neededIndex).GetComponent<ScenarioStage>();
                if (!stage.IsTransitionalStage)
                {
                    return transform.parent.GetChild(neededIndex).GetComponent<ScenarioStage>();
                }
            }
        }

        return null;
    }

    /*#region OnValidate
#if UNITY_EDITOR
    private void OnValidate()
    {
        bool isValidStage = false;

        if (BeforeStartStage == null)
        {
            BeforeStartStage = new UnityEvent();
        }

        if(OnStartStage == null)
        {
            OnStartStage = new UnityEvent();
        }

        if(OnEndStage == null)
        {
            OnEndStage = new UnityEvent();
        }

        if (OnStartStage.GetPersistentEventCount() > 0) 
        {
            var currentTarget = OnStartStage.GetPersistentTarget(OnStartStage.GetPersistentEventCount() - 1);
            var lastStageType = currentTarget.GetType();

            if(lastStageType == typeof(GameObject))
            {
                if(((GameObject)currentTarget).TryGetComponent<KeyObject>(out _))
                {
                    isValidStage = true;
                }
            }

            if(!isValidStage && (lastStageType == typeof(KeyObject) || lastStageType.BaseType == typeof(KeyObject)))
            {
                isValidStage = true;
            }

            if(!isValidStage && lastStageType != typeof(UnityEngine.Object))
            {
                EditorUtility.DisplayDialog("Type of object is wrong", "Please, choose object with KeyObject component", "OK"); 
            }
        }

        if(isValidStage && OnEndStage.GetPersistentEventCount() > 0)
        {
            isValidStage = false;

            var currentTarget = OnEndStage.GetPersistentTarget(OnEndStage.GetPersistentEventCount() - 1);
            var lastStageType = currentTarget.GetType();

            if (lastStageType == typeof(GameObject))
            {
                if (((GameObject)currentTarget).TryGetComponent<KeyObject>(out _))
                {
                    isValidStage = true;
                }
            }

            if (!isValidStage && (lastStageType == typeof(KeyObject) || lastStageType.BaseType == typeof(KeyObject)))
            {
                isValidStage = true;
            }

            if (!isValidStage && lastStageType != typeof(UnityEngine.Object))
            {
                EditorUtility.DisplayDialog("Type of object is wrong", "Please, choose object with KeyObject component", "OK");
            }
        }

        if (isValidStage && BeforeStartStage.GetPersistentEventCount() > 0)
        {
            isValidStage = false;

            var currentTarget = BeforeStartStage.GetPersistentTarget(BeforeStartStage.GetPersistentEventCount() - 1);
            var lastStageType = currentTarget.GetType();

            if (lastStageType == typeof(GameObject))
            {
                if (((GameObject)currentTarget).TryGetComponent<KeyObject>(out _))
                {
                    isValidStage = true;
                }
            }

            if (!isValidStage && (lastStageType == typeof(KeyObject) || lastStageType.BaseType == typeof(KeyObject)))
            {
                isValidStage = true;
            }

            if (!isValidStage && lastStageType != typeof(UnityEngine.Object))
            {
                EditorUtility.DisplayDialog("Type of object is wrong", "Please, choose object with KeyObject component", "OK");
            }
        }
    }
#endif
    #endregion*/
}

[Serializable]
public struct ScenarioStageStruct
{
    public bool IsTransitionalStage;
    public Vector3 Position;
    public Quaternion Rotation;
    public KeyObjectStruct KeyObject;
    public List<StageAction> BeforeStartStageActions;
    public List<StageAction> OnStartStageActions;
    public List<StageAction> BeforeEndStageActions;
    public List<StageAction> OnEndStageActions;
    public List<DescriptionWindowInfo> DescriptionsInfo;
    public List<GradientObjectInfo> GradientObjectsInfo;
    public string MainDialogWindowMessaage;
}

[Serializable]
public struct StageAction
{
    public string ObjectName;
    public string TypeName;
    public string ActionName;
}

[Serializable]
public struct DescriptionWindowInfo
{
    public string Name;
    public string Title;
    public string Message;
    public float[] Arguments;
    public float[] StepArguments;
    public float[] MaxValues;
    public bool[] LoopParams;
    public Vector3 Position;
    public Quaternion Rotation;
}

[Serializable]
public struct GradientObjectInfo
{
    public string Name;
    public Vector3 Position;
    public Quaternion Rotation;
    public bool IsIncreasing;
}

