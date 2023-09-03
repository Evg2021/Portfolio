using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [DllImport("PultControl.dll", EntryPoint = "OpenPult", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int fnOnOpenPult(string lpszPultName, string lpszWinddowName, int Type);

    [DllImport("PultControl.dll", EntryPoint = "ClosePult", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int fnOnClosePult(string lpszPultName);

    string pultName = "13mnbvc";

    public InputField field;
    public Material testMaterial;

    public void OnClickLoad()
    {
        var texture = Utilities.LoadTexture(field.text);
        if(texture != null)
        {
            testMaterial.SetTexture("_BaseMap", texture);
        }
    }


    private void OnDisable()
    {
    }

    private void OnDestroy()
    {
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(StartTest());
        }

    }*/

    private void Start()
    {
        /*var bytes = File.ReadAllBytes(Utilities.OpenFilePanel("txt"));
        int byteIdx = 0;
        var word = "";

        int firstParamSize = bytes[byteIdx];
        byteIdx += 4;

        for(int i = 0; i < firstParamSize; i++)
        {
            word += Convert.ToChar(bytes[byteIdx++]);
        }

        Debug.Log(word);*/
    }

    private IEnumerator StartTest()
    {
        var imagenames = GetAllImagenames(Utilities.OpenBrowserPanel());

        for(int i = 1; i <= imagenames.Count(); i++)
        {
            UIController.Instance.OnClickAddArrow();
            var currentArrow = UIController.Instance.CurrentArrow.GetComponent<Arrow>();
            currentArrow.Approve(imagenames[i]);
            AdapterPanelController.Instance.OnClickSubmit();
            currentArrow.OnPressDown();
            currentArrow.OnPressUp();
            yield return new WaitForEndOfFrame();
        }
    }

    private string[] GetAllImagenames(string path)
    {
        var imagenames = Directory.GetFiles(path, "*.jpg");
        Debug.Log(imagenames.Length);
        return imagenames;
    }
}
