using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ValveControllerTest : MonoBehaviour
{
    public enum Types : ushort
    {
        TYPE_UNKNOWN,
        TYPE_DOUBLE,
        TYPE_INT,
        TYPE_FLOAT,
        TYPE_STRING,
        TYPE_BOOL
    };



    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "RegistrateParameter")]
    unsafe public static extern UInt32 RegistrateParameter(String ObjName, String ParamName, Types TypeValueGet, void* PointValueGet, Types TypeValueSet, void* PointValueSet);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGetOne")]
    public static extern void UpdateGetOne(UInt32 index);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateSetOne")]
    public static extern void UpdateSetOne(UInt32 index);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGet")]
    public static extern void UpdateGet(UInt32[] array, UInt32 size);

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateGetAll")]
    public static extern void UpdateGetAll();

    [DllImport("ClientSocket.dll", CharSet = CharSet.Auto, EntryPoint = "UpdateSetAll")]
    public static extern void UpdateSetAll();

    

    public uint ownIndex;

    public Transform ObjectToRotate;
    public Text TextFromDLL;
    public Text TrueText;
    public InputField InputField;
    public Text RealDelay;

    private float currentPercent;
    private float currentAngle;

    private float minAngle = 0.0f;
    private float maxAngle = 720.0f;

    private string ObjName = "13";
    private string ParamName = "#Положение";


    private float startValue = 0.0f;
    private float delay = 0.2f;
    private double delayText;

    public bool isInitialized;
    public bool allowAsyncRequesting;
    private bool allowToRequest;

    private float getValue;
    private float setValue;

    public bool allowSending = true;

    public void Launch()
    {
        if (InputField)
        {
            InputField.text = (delay * 1000).ToString();
        }

        Initialize();
    }

    public void Initialize()
    {
        isInitialized = false;
        allowToRequest = false;
        currentAngle = 0.0f;

        try
        {
            isInitialized = true;
            allowToRequest = true;

            RegistrateObject();
            StartCoroutine(GetParams());

            //Task.Run(StartCreatingRequests);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    private void RegistrateObject()
    {
        unsafe
        {
            fixed (float* set = &setValue, get = &getValue)
            {
                ownIndex = RegistrateParameter(ObjName, ParamName, Types.TYPE_FLOAT, get, Types.TYPE_FLOAT, set);

                if (ownIndex == uint.MaxValue)
                {
                    Debug.LogError("Object " + ObjName + " was not registrated.");
                }
                else
                {
                    Debug.Log("Object " + ObjName + " was registred with index: " + ownIndex);
                }
            }
        }
    }
    
    private void Update()
    {
        if (TrueText)
        {
            TrueText.text = setValue.ToString("00.000") + "%";           
        }

//        CreateRequest();
    }

    private void CreateRequest()
    {
        //UpdateGetOne(ownIndex);

        if (TextFromDLL)
        {
            TextFromDLL.text = getValue.ToString("00.000") + '%';
        }

        Debug.Log("Request.");
    }

    private IEnumerator GetParams()
    {
        delayText = 0.0f;
        while (allowToRequest)
        {
            double currentTime = Time.realtimeSinceStartup;
            CreateRequest();

            yield return new WaitForSeconds(delay);

            delayText = Time.realtimeSinceStartup - currentTime;
            if (RealDelay)
            {
                RealDelay.text = "Время задержки: " + delayText.ToString("0.000");
                delayText = 0.0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isInitialized)
        {
            if (Input.GetKey(KeyCode.A))
            {
                SetRotation(currentPercent - 1);
            }

            if (Input.GetKey(KeyCode.D))
            {
                SetRotation(currentPercent + 1);
            }
        }
    }

    public void Rotate(float value)
    {
        if (value != currentPercent)
        {
            var filteredValue = Mathf.Clamp(value, 0, 100);
            float newAngle = Mathf.Lerp(minAngle, maxAngle, (float)filteredValue / 100);

            if (ObjectToRotate)
            {
                currentAngle = newAngle;
                currentPercent = filteredValue;
                ObjectToRotate.Rotate(Vector3.up, newAngle);
            }
        }
    }

    private void SetRotation(float value)
    {
        if (!allowSending) return;

        Rotate(value);
        setValue = currentPercent;

        try
        {
            UpdateSetOne(ownIndex);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetDelay(string value)
    {
        float number = int.Parse(value) * 0.001f;
        delay = Mathf.Abs(number);
    }
}
