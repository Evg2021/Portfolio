using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniStorm;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : SingletonMonoBehaviour<PauseMenuController>
{
    private GameObject[] children;
    private Image background;

    private bool isPaused = false;

    public PlayerController ownerController;
    private UniStormSystem uniStorm; 

    protected override void Awake()
    {
        base.Awake();
        InitializeChildren();
        InitializeBackground();
        
        HidePauseMenu();
    }

    private void Start()
    {
        InitializeUniStormSystem();
    }

    private void InitializeChildren()
    {
        int childCount = transform.childCount;
        children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
    }

    private void InitializeBackground()
    {
        background = GetComponent<Image>();
    }

    private void InitializeUniStormSystem()
    {
        uniStorm = FindObjectOfType<UniStormSystem>();
    }
       
    public void OnClickContinueButton()
    {
        if (ownerController)
        {
            ownerController.SetPause();
        }
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void ShowPauseMenu()
    {
        SetActivePauseMenu(true);
        isPaused = true;
    }

    public void HidePauseMenu()
    {
        SetActivePauseMenu(false);
        isPaused = false;
    }

    private void SetActivePauseMenu(bool value)
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetActive(value);
        }

        if (background)
        {
            background.enabled = value;
        }   
    }
}
