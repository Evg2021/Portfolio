using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueFloatShowerController : TrenObjectControllerBase
{
    public bool isPersented = true;
    public bool isInitilaize = false;

    private TextMeshProUGUI text;

    private unsafe float* getFloat;
    private unsafe float* setFloat;

    public unsafe float SimulatorValue { get { return *getFloat; } }

    public override unsafe void Initialize(uint index, void* get, void* set)
    {
        base.Initialize(index, get, set);
        getFloat = (float*)get;
        setFloat = (float*)set;
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
                if (isPersented)
                {
                    text.text = (*getFloat).ToString("F2") + '%';
                }
                else
                {
                    text.text = (*getFloat).ToString("F2");
                }
            }
        }
    }
}
