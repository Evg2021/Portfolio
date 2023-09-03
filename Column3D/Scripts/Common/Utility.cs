using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    public static string UIPrefabsFolder               = "Prefabs\\UI\\";
    public static string DescriptionWindow3DPrefabName = "Description3D";
    public static string DescriptionWindow2DPrefabName = "Description2D";
    public static string DialogWindowItemPrefabName    = "DialogItem";
    public static string GradientPrefabName            = "Gradient";
    public static string ScenarioButtonPrefabName      = "ScenarioButton";

    private static string canvas3DName = "Canvas3D";
    private static string canvas2DName = "Canvas2D";

    private static string pattern = @"(?<!\{)(?>\{\{)*\{\d(.*?)";

    private static GameObject canvas3D;
    private static GameObject canvas2D;

    public static void AddVoidPersistentListener(UnityEvent unityEvent, object target, string methodName)
    {
        var unityAction = Delegate.CreateDelegate(typeof(UnityAction), target, methodName) as UnityAction;

        bool eventWasAdded = false;
#if UNITY_EDITOR
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(unityEvent, unityAction);
        eventWasAdded = true;
#endif
        if (!eventWasAdded)
        {
            unityEvent.AddListener(unityAction);
        }

    }
    public static void InitializeScenarioStage(ScenarioStage stage, ScenarioStageStruct info)
    {
        stage.SetBeforeStartStage  (info.BeforeStartStageActions);
        stage.SetStartStage        (info.OnStartStageActions);
        stage.SetBeforeEndStage    (info.BeforeEndStageActions);
        stage.SetEndStage          (info.OnEndStageActions);
        stage.SetKeyObject         (info.KeyObject);
        stage.SetDescriptionWindows(info.DescriptionsInfo);
        stage.SetGradientObjects   (info.GradientObjectsInfo);
        stage.SetMainMessage       (info.MainDialogWindowMessaage);
        stage.SetTransitionalStatus(info.IsTransitionalStage, info.Position, info.Rotation);
    }

    public static KeyObjectStruct GetKeyObjectStruct(KeyObject keyObject)
    {
        if (keyObject != null)
        {
            return new KeyObjectStruct()
            {
                Name = keyObject.name,
                Description = GetKeyObjectDescriptionInfo(keyObject),
                Track = GetTrack(keyObject)
            };
        }

        return default(KeyObjectStruct);
    }
    public static KeyObjectDescriptionWindow GetKeyObjectDescriptionInfo(KeyObject keyObject)
    {
        return new KeyObjectDescriptionWindow()
        {
            Title = keyObject.Title,
            Message = keyObject.Description
        };
    }
    public static Track GetTrack(KeyObject keyObject)
    {
        return new Track()
        {
            ViewPosition  = keyObject.ViewPosition,
            ViewRotation  = keyObject.ViewRotation
        };
    }
    public static List<StageAction> GetStageActions(UnityEvent unityEvent)
    {
        var result = new List<StageAction>();
        var countEvents = unityEvent.GetPersistentEventCount();
        
        for(int i = 0; i < countEvents; i++)
        {
            var target = unityEvent.GetPersistentTarget(i);
            var actionName = unityEvent.GetPersistentMethodName(i);

            var stageAction = new StageAction()
            {
                ObjectName = target.name,
                TypeName = target.GetType().ToString(),
                ActionName = actionName
            };

            result.Add(stageAction);
        }

        return result;
    }
    public static ScenarioStageStruct GetScenarioStageStruct(ScenarioStage scenarioStage)
    {
        return new ScenarioStageStruct()
        {
            KeyObject = GetKeyObjectStruct(scenarioStage.KeyObject),
            BeforeStartStageActions = GetStageActions(scenarioStage.BeforeStartStage),
            OnStartStageActions = GetStageActions(scenarioStage.OnStartStage),
            BeforeEndStageActions = GetStageActions(scenarioStage.BeforeEndStage),
            OnEndStageActions = GetStageActions(scenarioStage.OnEndStage),
            DescriptionsInfo = GetDescriptionWindowsInfo(scenarioStage.DescriptionWindows),
            GradientObjectsInfo = GetGradientObjectsInfo(scenarioStage.GradientObjects),
            MainDialogWindowMessaage = scenarioStage.MainWindowMessage,
            IsTransitionalStage = scenarioStage.IsTransitionalStage,
            Position = scenarioStage.transform.position,
            Rotation = scenarioStage.transform.rotation
        };
    }
    public static DescriptionWindowInfo GetDescriptionWindowInfo(DescriptionController controller)
    {
        GetDescriptionWindowDynamicArguments(controller.Arguments, out var arguments, out var stepArguments, out var maxValues);

        return new DescriptionWindowInfo()
        {
            Name = controller.name,
            Message = controller.Message,
            Title = controller.Title,
            Arguments = arguments,
            StepArguments = stepArguments,
            MaxValues = maxValues,
            LoopParams = controller.IsLoop,
            Position = controller.transform.position,
            Rotation = controller.transform.rotation
        };
    }
    public static GradientObjectInfo GetGradientObjectInfo(GradientController gradient)
    {
        return new GradientObjectInfo()
        {
            Name = gradient.name,
            Position = gradient.transform.position,
            Rotation = gradient.transform.rotation,
            IsIncreasing = gradient.IsIncreasing
        };
    }
    public static List<DescriptionWindowInfo> GetDescriptionWindowsInfo(List<DescriptionController> windows)
    {
        var result = new List<DescriptionWindowInfo>();

        if (windows != null)
        {
            foreach (var window in windows)
            {
                var info = GetDescriptionWindowInfo(window);
                result.Add(info);
            }
        }

        return result;
    }
    public static List<GradientObjectInfo> GetGradientObjectsInfo(List<GradientController> gradients)
    {
        var result = new List<GradientObjectInfo>();

        if (gradients != null)
        {
            foreach (var gradient in gradients)
            {
                var info = GetGradientObjectInfo(gradient);
                result.Add(info);
            }
        }

        return result;
    }
    public static void GetDescriptionWindowDynamicArguments(float[,] dynamicArguments, out float[] arguments, out float[] steps, out float[] maxValues)
    {
        arguments = null;
        steps     = null;
        maxValues = null;

        if (dynamicArguments != null && dynamicArguments.GetLength(0) > 0)
        {
            int count = dynamicArguments.GetLength(0);
            arguments = new float[count];
            steps     = new float[count];
            maxValues = new float[count];

            for (int i = 0; i < dynamicArguments.GetLength(0); i++)
            {
                arguments[i] = dynamicArguments[i, 0];
                steps[i]     = dynamicArguments[i, 1];
                maxValues[i] = dynamicArguments[i, 2];
            }
        }
    }
         
    public static GameObject GetDescriptionWindow3DPrefab()
    {
        var prefab = Resources.Load(UIPrefabsFolder + DescriptionWindow3DPrefabName, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("Description Window Prefab is missing in \"" + UIPrefabsFolder + "\" folder with name \"" + DescriptionWindow3DPrefabName + "\"");
            return null;
        }

        return prefab;
    }
    public static GameObject GetDescriptionWindow2DPrefab()
    {
        var prefab = Resources.Load(UIPrefabsFolder + DescriptionWindow2DPrefabName, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("Description Window Prefab is missing in \"" + UIPrefabsFolder + "\" folder with name \"" + DescriptionWindow2DPrefabName + "\"");
            return null;
        }

        return prefab;
    }
    public static GameObject GetDialogWindowItemPrefab()
    {
        var prefab = Resources.Load(UIPrefabsFolder + DialogWindowItemPrefabName, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("Dialog Item Prefab is missing in \"" + UIPrefabsFolder + "\" folder with name \"" + DialogWindowItemPrefabName + "\"");
            return null;
        }

        return prefab;
    }
    public static GameObject GetGradientObjectPrefab()
    {
        var prefab = Resources.Load(UIPrefabsFolder + GradientPrefabName, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("GradientObject Prefab is missing in \"" + UIPrefabsFolder + "\" folder with name \"" + GradientPrefabName + "\"");
            return null;
        }

        return prefab;
    }
    public static GameObject GetScenarioButtonPrefab()
    {
        var prefab = Resources.Load(UIPrefabsFolder + ScenarioButtonPrefabName, typeof(GameObject)) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("GradientObject Prefab is missing in \"" + UIPrefabsFolder + "\" folder with name \"" + ScenarioButtonPrefabName + "\"");
            return null;
        }

        return prefab;
    }

    public static GameObject GetCanvas3D()
    {
        if (canvas3D == null)
        {
            canvas3D = GameObject.Find(canvas3DName);
            if (canvas3D == null)
            {
                Debug.LogError("Canvas with World space and name \"" + canvas3DName + "\" is missnig on scene.");
                return null;
            }
        }

        return canvas3D;
    }
    public static GameObject GetCanvas2D()
    {
        if (canvas2D == null)
        {
            canvas2D = GameObject.Find(canvas2DName);
            if (canvas2D == null)
            {
                Debug.LogError("Canvas with World space and name \"" + canvas2DName + "\" is missnig on scene.");
                return null;
            }
        }

        return canvas2D;
    }

    public static int GetStringParamsCount(string input)
    {
        var matches = Regex.Matches(input, pattern);
        return matches.Count;
    }
}
