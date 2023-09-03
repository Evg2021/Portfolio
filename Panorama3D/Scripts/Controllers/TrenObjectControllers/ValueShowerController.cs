using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValueShowerController : TrenObjectControllerBase
{
    private TextMeshProUGUI text;

    private unsafe float* getFloat;
    private unsafe float* setFloat;

    private bool isInitilaize = false;

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
                text.text = (*getFloat).ToString() + '%';
            }
        }
    }
}
