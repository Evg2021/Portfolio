using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerController : MonoBehaviour
{
    public uint Index;
    public bool test = false;
    public bool registred = false;

    public string objName { get; private set; }
    public string paramName { get; private set; }
    private MeshRenderer renderer;
    private Material origianlMaterial;

    private float get;
    private float set;

    private void Awake()
    {
        if (TryGetComponent(out renderer))
        {
            origianlMaterial = renderer.material;
        }
    }

    public void Initialize(string objName, string paramName)
    {
        this.objName = objName;
        this.paramName = paramName;
    }

    public void Test()
    {
        unsafe
        {
            fixed (float* getPtr = &get, setPtr = &set)
            {
                Index = ClientSocketManager.RegistrateParameter(objName, paramName, Types.TYPE_FLOAT, getPtr, Types.TYPE_FLOAT, setPtr);
            }
        }

        if (Index != uint.MaxValue)
        {
            renderer.material = GlobalVariables.RightHighlightMaterial;
            registred = true;
        }
        else
        {
            renderer.material = GlobalVariables.WrongBindHighlightMaterial;
        }
    }
    public void DisableTest()
    {
        if (renderer)
        {
            renderer.material = origianlMaterial;
        }
    }

    private void OnDestroy()
    {
        if (registred)
        {
            ClientSocketManager.RemoveRegistratedParameter(Index);
        }
    }
}
