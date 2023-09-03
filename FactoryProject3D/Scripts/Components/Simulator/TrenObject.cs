using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class TrenObject : MonoBehaviour, ITrenInteractable
{
    public string TrenName { get; private set; }
    public string TrenParameter { get; private set; }
    public ObjectMode ObjectMode { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsRegistrated { get; protected set; }

    public uint Index;// { get; protected set; }

    private void Awake()
    {
        IsInitialized = false;
        IsRegistrated = false;
    }

    public void Initialize(string trenName, string trenParameter, ObjectMode mode = ObjectMode.SETGET)
    {
        TrenName = trenName;
        TrenParameter = trenParameter;
        ObjectMode = mode;
        IsInitialized = true;
    }

    public virtual void UpdateGetter()
    {
        if (Index != uint.MaxValue)
        {
            ClientSocketManager.UpdateGetOne(Index);
        }
    }
    public virtual void UpdateSetter()
    {
        if (Index != uint.MaxValue)
        {
            ClientSocketManager.UpdateSetOne(Index);
        }
    }

    public abstract void RegistrateObject();

    public ObjectMode GetObjectMode()
    {
        return ObjectMode;
    }

    public bool IsObjectInitialized()
    {
        return IsInitialized;
    }

    public bool IsObjectRegistrated()
    {
        return IsRegistrated;
    }

    public string GetTrenName()
    {
        return TrenName;
    }

    public uint GetTrenIndex()
    {
        return Index;
    }
}

public enum ObjectMode
{
    SET    = 0,
    GET    = 1,
    SETGET = 2
}

[Serializable]
public struct TrenObjectData
{
    public string UnityName;
    public string TrenName;
    public string TrenParameter;
    public ObjectMode ObjectMode;

    public static bool operator ==(TrenObjectData data1, TrenObjectData data2)
    {
        return data1.Equals(data2);
    }
    public static bool operator !=(TrenObjectData data1, TrenObjectData data2)
    {
        return !data1.Equals(data2);
    }
}

[Serializable]
public struct TrenObjectDataList
{
    public List<TrenObjectData> Data;
}