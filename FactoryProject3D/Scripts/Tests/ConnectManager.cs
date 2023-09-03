using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ConnectManager : MonoBehaviour
{
    public delegate void callback([MarshalAs(UnmanagedType.LPWStr)] IntPtr ObjectName, [MarshalAs(UnmanagedType.LPWStr)] IntPtr DefectName);

    

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "CreateConnect")]
    private static extern int CreateConnect(string ip, UInt16 Port, callback back);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "ClearRegistrattion")]
    public static extern void ClearRegistration();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "CloseConnect")]
    public static extern int CloseConnect();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGetAll")]
    public static extern void UpdateGetAll();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Ansi, EntryPoint = "TestBack", CallingConvention = CallingConvention.Cdecl)]
    public static extern void TestBack(callback back);

    private static string IPAddress;
    private string DefaultIPAddress = "192.168.111.129"; 

    public InputField IPInput;
    public Text ValvesCount;
    public Text textFromDll;

    private void Update()
    {
        try
        {
            UpdateGetAll();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TestBack(SetTextFromDLL);
        }*/
    }

    public void SetTextFromDLL(IntPtr objName, IntPtr defectName)
    {
        Debug.Log("ObjectName: " + objName);
        Debug.Log("DefectName: " + defectName);

        string encodedObjectName = Utilities.GetStringFromIntPtr(objName, Encoding.GetEncoding("windows-1251"));
        string encodedDefectName = Utilities.GetStringFromIntPtr(defectName, Encoding.GetEncoding("windows-1251"));

        if (textFromDll)
        {
            textFromDll.text = $"Object Name: {encodedObjectName}\nDefect Name: {encodedDefectName}";
        }
    }

    

    // Start is called before the first frame update
    void Awake()
    {
        if (string.IsNullOrEmpty(IPAddress))
        {
            IPAddress = DefaultIPAddress;
        }

        if (IPInput)
        {
            IPInput.text = IPAddress;
        }

        int create = CreateConnect(IPAddress, 20200, SetTextFromDLL);

        if (create == 0)
        {
            var valves = FindObjectsOfType<ValveControllerTest>();
            foreach (var valve in valves)
            {
                valve.Launch();
            }

            if (ValvesCount)
            {
                ValvesCount.text = valves.Length.ToString();
            }
        }

        Debug.Log("On Start Create: " + create);
    }

    private void OnDisable()
    {
        try
        {
            ClearRegistration();
            CloseConnect();
            Debug.Log("Disconnected successfuly.");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void OnSubmitClick()
    {
        if (IPInput && !string.IsNullOrEmpty(IPInput.text))
        {
            IPAddress = IPInput.text;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.5f, 10, 100, 50), "FPS: " + 1.0f / Time.deltaTime);
    }
}

