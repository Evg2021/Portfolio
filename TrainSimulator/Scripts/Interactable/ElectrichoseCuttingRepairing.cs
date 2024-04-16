using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrichoseCuttingRepairing : MonoBehaviour
{
    [SerializeField] private Transform _leftElements;
    [SerializeField] private Transform _rightElements;
    [SerializeField] private Transform _rightParent;
    [SerializeField] private Transform _leftParent;
    
    [Space(10)]
    [SerializeField] private GameObject _hose1_default;
    [SerializeField] private GameObject _hose2_default;
    [SerializeField] private GameObject _hose3_default;
    
    [Space(10)]
    [SerializeField] private GameObject _hose1_cut;
    [SerializeField] private GameObject _hose2_cut;
    [SerializeField] private GameObject _hose3_cut;
    
    [Space(10)]
    [SerializeField] private GameObject _hose1_repair;
    [SerializeField] private GameObject _hose2_repair;
    [SerializeField] private GameObject _hose3_repair;


    [ContextMenu("Child separate")]
    public void ChildSeparate()
    {
        _leftElements.parent = _leftParent;
        _rightElements.parent = _rightParent;
    }
    
    [ContextMenu("Child return")]
    public void ChildReturn()
    {
        _leftElements.parent = transform;
        _rightElements.parent = transform;
    }
    
    [ContextMenu("Cut all")]
    public void CutAll()
    {
        _hose1_default.SetActive(false);
        _hose2_default.SetActive(false);
        _hose3_default.SetActive(false);
        
        _hose1_repair.SetActive(false);
        _hose2_repair.SetActive(false);
        _hose3_repair.SetActive(false);
        
        _hose1_cut.SetActive(true);
        _hose2_cut.SetActive(true);
        _hose3_cut.SetActive(true);
    }
    
    [ContextMenu("Repair all")]
    public void RepairAll()
    {
        Repair1();
        Repair2();
        Repair3();
    }
    
    [ContextMenu("RESET")]
    public void RESET()
    {
        _hose1_default.SetActive(true);
        _hose2_default.SetActive(true);
        _hose3_default.SetActive(true);
        
        _hose1_repair.SetActive(false);
        _hose2_repair.SetActive(false);
        _hose3_repair.SetActive(false);
        
        _hose1_cut.SetActive(false);
        _hose2_cut.SetActive(false);
        _hose3_cut.SetActive(false);
    }
    [ContextMenu("Show all")]
    public void ShowAll()
    {
        _hose1_default.SetActive(true);
        _hose2_default.SetActive(true);
        _hose3_default.SetActive(true);
        
        _hose1_repair.SetActive(true);
        _hose2_repair.SetActive(true);
        _hose3_repair.SetActive(true);
        
        _hose1_cut.SetActive(true);
        _hose2_cut.SetActive(true);
        _hose3_cut.SetActive(true);
    }
    [ContextMenu("Repair 1")]
    public void Repair1()
    {
        _hose1_default.SetActive(false);
        _hose1_repair.SetActive(true);
        _hose1_cut.SetActive(false);
    }
    
    [ContextMenu("Repair 2")]
    public void Repair2()
    {
        _hose2_default.SetActive(false);
        _hose2_repair.SetActive(true);
        _hose2_cut.SetActive(false);
    }
    
    [ContextMenu("Repair 3")]
    public void Repair3()
    {
        _hose3_default.SetActive(false);
        _hose3_repair.SetActive(true);
        _hose3_cut.SetActive(false);
    }
    
}
