using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ClientSocketManager
{
    public delegate void callback([MarshalAs(UnmanagedType.LPWStr)] IntPtr ObjectName, [MarshalAs(UnmanagedType.LPWStr)] IntPtr DefectName);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "RegistrateParameter")]
    unsafe private static extern UInt32 fnRegistrateParameter(String ObjName, String ParamName, Types TypeValueGet, void* PointValueGet, Types TypeValueSet, void* PointValueSet);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "CreateConnect")]
    private static extern int fnCreateConnect(string ip, UInt16 Port, callback back);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "ClearRegistration")]
    private static extern void fnClearRegistration();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "RemoveRegistratedParameter")]
    private static extern void fnRemoveRegistratedParameter(UInt32 index);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "CloseConnect")]
    private static extern int fnCloseConnect();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGetOne")]
    private static extern void fnUpdateGetOne(UInt32 index);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateSetOne")]
    private static extern void fnUpdateSetOne(UInt32 index);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGet")]
    private static extern void fnUpdateGet(UInt32[] array, UInt32 size);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGetAll")]
    private static extern void fnUpdateGetAll();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateSetAll")]
    private static extern void fnUpdateSetAll();

    unsafe public static uint RegistrateParameter(string objName, string paramName, Types typeValueGet, void* pointValueGet, Types typeValueSet, void* pointValueSet)
    {
        try
        {
            return fnRegistrateParameter(objName, paramName, typeValueGet, pointValueGet, typeValueSet, pointValueSet);
        }
        catch (Exception e)
        {
            Debug.LogError($"{objName} registration with {paramName} parameter failed: " + e.Message);
            return uint.MaxValue;
        }
    }
    public static int CreateConnect(string ip, ushort port, callback callbackFunction)
    {
        try
        {
            return fnCreateConnect(ip, port, callbackFunction);
        }
        catch (Exception e)
        {
            Debug.LogError("Creating connection error: " + e.Message);
            return -1;
        }
    }
    public static void ClearRegistration()
    {
        try
        {
            fnClearRegistration();
        }
        catch (Exception e)
        {
            Debug.LogError("Clearing registration error: " + e.Message);
        }
    }
    public static void RemoveRegistratedParameter(uint index)
    {
        try
        {
            fnRemoveRegistratedParameter(index);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public static int CloseConnect()
    {
        try
        {
            return fnCloseConnect();
        }
        catch (Exception e)
        {
            Debug.LogError("Closing connection error: " + e.Message);
            return -1;
        }
    }
    public static void UpdateGetOne(uint index)
    {
        try
        {
            fnUpdateGetOne(index);
        }
        catch (Exception e)
        {
            Debug.LogError("Updating on get one error: " + e.Message);
        }
    }
    public static void UpdateSetOne(uint index)
    {
        try
        {
            fnUpdateSetOne(index);
        }
        catch (Exception e)
        {
            Debug.LogError("Updating on set one error: " + e.Message);
        }
    }
    public static void UpdateGet(uint[] indices, uint indicesCount)
    {
        try
        {
            fnUpdateGet(indices, indicesCount);
        }
        catch (Exception e)
        {
            Debug.LogError("Updating on get array error: " + e.Message);
        }
    }
    public static void UpdateGetAll()
    {
        try
        {
            fnUpdateGetAll();
        }
        catch (Exception e)
        {
            Debug.LogError("Updating on get all error: " + e.Message);
        }
    }
    public static void UpdateSetAll()
    {
        try
        {
            fnUpdateSetAll();
        }
        catch (Exception e)
        {
            Debug.LogError("Updating on set all error: " + e.Message);
        }
    }
}

public enum Types : ushort
{
    TYPE_UNKNOWN,
    TYPE_DOUBLE,
    TYPE_INT,
    TYPE_FLOAT,
    TYPE_STRING,
    TYPE_BOOL
};
