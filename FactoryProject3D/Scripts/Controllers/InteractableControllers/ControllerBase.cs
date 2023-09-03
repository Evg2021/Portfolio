using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour, IController
{
    public bool isEnabled { get; protected set; }

    public abstract dynamic GetSimulatorValue();
    public abstract string GetTrenName();
    public abstract void Initialize();
    public abstract void Interact(dynamic value, bool withSimulator = true);

    public abstract Types GetControllerType();
    public virtual int GetTypesCount()
    {
        return 1;
    }

    public abstract uint GetTrenObjectIndex();

    public virtual void Enable()
    {
        if (!isEnabled)
        {
            isEnabled = true;
        }
    }
    public virtual void Disable()
    {
        if (isEnabled)
        {
            isEnabled = false;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    /// <summary>
    /// Initialization of TrenObjects. First parameter Set, second - get.
    /// </summary>
    /// <typeparam name="T">Type of TrenObject kind of ITrenInteractable</typeparam>
    /// <param name="trenObjectSet"></param>
    /// <param name="trenObjectGet"></param>
    /// <returns></returns>
    protected bool InitializeTrenObject<T, U>(out T trenObjectSet, out U trenObjectGet)
        where T : ITrenInteractable
        where U : ITrenInteractable
    {
        trenObjectSet = default(T);
        trenObjectGet = default(U);

        bool trenObjectSetInitialized = false;
        bool trenObjectGetInitialized = false;

        var currentTrenObjectsT = GetComponents<T>();
        var currentTrenObjectsU = GetComponents<U>();

        if (currentTrenObjectsT == null || currentTrenObjectsT.Length == 0 || currentTrenObjectsU == null || currentTrenObjectsU.Length == 0)
        {
            Debug.LogWarning($"TrenObject was not found in {transform.name} object.");
        }
        else
        {
            foreach (var trenObject in currentTrenObjectsT)
            {
                if (trenObject.IsObjectRegistrated())
                {
                    if (trenObject.GetObjectMode() == ObjectMode.SET || trenObject.GetObjectMode() == ObjectMode.SETGET)
                    {
                        trenObjectSet = trenObject;
                        trenObjectSetInitialized = true;
                        break;
                    }
                }
            }

            foreach (var trenObject in currentTrenObjectsU)
            {
                if (trenObject.IsObjectRegistrated())
                {
                    if (trenObject.GetObjectMode() == ObjectMode.GET || trenObject.GetObjectMode() == ObjectMode.SETGET)
                    {
                        trenObjectGet = trenObject;
                        trenObjectGetInitialized = true;
                        break;
                    }
                }
            }
        }

        return trenObjectGetInitialized && trenObjectSetInitialized;
    }
}

public interface IController
{
    public dynamic GetSimulatorValue();
    public string GetTrenName();
    public uint GetTrenObjectIndex();
    public void Interact(dynamic value, bool withSimulator = true);
    public void Initialize();
    public Types GetControllerType();
    public int GetTypesCount();
    public Transform GetTransform();
}

public interface ICustomInfoController
{
    public string GetPostfix();
}

public interface ITrenObjectManipulator
{
    public void UpdateTrenObject();
}