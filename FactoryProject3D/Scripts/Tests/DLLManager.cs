using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DLLManager : MonoBehaviour
{
    [DllImport("kernel32.dll", EntryPoint = "SetDllDirectory", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetDllDirectory(string strDllPath);

    public GameObject Panel;
    public InputField InputField;

    public void Update()
    {
        if (Panel && Panel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            Apply();
        }
    }

    public void Apply()
    {
        if (Panel && InputField && !string.IsNullOrEmpty(InputField.text))
        {
            if (SetDllDirectory(InputField.text))
            {
                Debug.Log("Path to DLL was changed successfuly.");

                Panel.SetActive(false);
                FindObjectOfType<ValveControllerTest>().Initialize();
            }
            else
            {
                Debug.LogError("Path to DLL was not changed.");
            }
        }

        if (InputField && string.IsNullOrEmpty(InputField.text))
        {
            Debug.LogError("Path should not empty.");
        }
    }
}
