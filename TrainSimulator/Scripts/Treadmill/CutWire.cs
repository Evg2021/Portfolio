using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutWire : MonoBehaviour
{
    [SerializeField] private GameObject _wire;
    [SerializeField] private GameObject _cutWire;

    public void Cut()
    {
        _wire.SetActive(false);
    }

    public void Restore()
    {
        _wire.SetActive(true);
    }

    public GameObject GetCutWirePoint()
    {
        return _cutWire;
    }
}
