using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HosepipeCuttingRepairing : MonoBehaviour
{
    [SerializeField] private GameObject _blueEndValveRight;
    [SerializeField] private GameObject _blueEndValveLeft;
    [SerializeField] private GameObject _redEndValveRight;
    [SerializeField] private GameObject _redEndValveLeft;
    [Space(10)] 
    [SerializeField] private GameObject _blueDefaultRight;
    [SerializeField] private GameObject _blueDefaultLeft;
    [SerializeField] private GameObject _redDefaultRight;
    [SerializeField] private GameObject _redDefaultLeft;
    [Space(10)] 
    [SerializeField] private GameObject _blueCutRight;
    [SerializeField] private GameObject _blueCutLeft;
    [SerializeField] private GameObject _redCutRight;
    [SerializeField] private GameObject _redCutLeft;

    [Space(10)] [SerializeField] private Transform _lestSide;
    [SerializeField] private Transform _rightSide;

    [ContextMenu("RepairAll")]
    public void RepairAll()
    {
        RepairBlue();
        RepairRed();
    }

    [ContextMenu("Cut all")]
    public void CutAll()
    {
        CutBlue();
        CutRed();
    }

    [ContextMenu("Repair blue")]
    public void RepairBlue()
    {
        _blueCutLeft.SetActive(false);
        _blueCutRight.SetActive(false);

        _blueDefaultLeft.SetActive(true);
        _blueDefaultRight.SetActive(true);
    }

    [ContextMenu("Repair red")]
    public void RepairRed()
    {
        _redCutRight.SetActive(false);
        _redCutLeft.SetActive(false);

        _redDefaultLeft.SetActive(true);
        _redDefaultRight.SetActive(true);
    }

    [ContextMenu("Cut blue")]
    public void CutBlue()
    {
        _blueCutRight.SetActive(true);
        _blueCutLeft.SetActive(true);

        _blueDefaultLeft.SetActive(false);
        _blueDefaultRight.SetActive(false);
    }

    [ContextMenu("Cut red")]
    public void CutRed()
    {
        _redCutLeft.SetActive(true);
        _redCutRight.SetActive(true);

        _redDefaultLeft.SetActive(false);
        _redDefaultRight.SetActive(false);
    }

    [ContextMenu("Childs separate")]
    public void ChildSeparate()
    {
        _redEndValveRight.transform.parent = _rightSide;
        _redEndValveLeft.transform.parent = _lestSide;
        
        _blueEndValveRight.transform.parent = _rightSide;
        _blueEndValveLeft.transform.parent = _lestSide;
    }

    [ContextMenu("Childs return")]
    public void ChildReturn()
    {
        _redEndValveRight.transform.parent = transform;
        _redEndValveLeft.transform.parent = transform;
        
        _blueEndValveRight.transform.parent = transform;
        _blueEndValveLeft.transform.parent = transform;
    }
}