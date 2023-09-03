using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueBoolShowerController : TrenObjectControllerBase
{    
    public bool isInitilaize = false;
    public string PositiveMessage = "Включен";
    public string NegativeMessage = "Выключен";

    private TextMeshProUGUI text;

    private unsafe bool* getBool;
    private unsafe bool* setBool;

    public unsafe bool SimulatorValue { get { return *getBool; } }

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);
        getBool = (bool*)get;
        setBool = (bool*)set;
        ClientSocketManager.UpdateGetOne(Index);
        isInitilaize = true;
    }

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isInitilaize && text)
        {
            unsafe
            {
                ClientSocketManager.UpdateGetOne(Index);                
                if (*getBool)
                {
                    text.text = PositiveMessage.ToString();
                }
                else
                {
                    text.text = NegativeMessage.ToString();
                }
            }
        }
    }
}
