using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueIntShowerController : TrenObjectControllerBase
{    
    public bool isInitilaize = false;
    public string LocalMessage = "Местный";
    public string DistantMessage = "Дистанционный";    

    private TextMeshProUGUI text;

    private unsafe int* getInt;
    private unsafe int* setInt;

    public unsafe int SimulatorValue { get { return *getInt; } }

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);
        getInt = (int*)get;
        setInt = (int*)set;
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
                
                if(*getInt == 0)
                {
                    text.text = DistantMessage.ToString();
                }
                if(*getInt == 1)
                {
                    text.text = LocalMessage.ToString();
                }
                
            }
        }
    }
}
