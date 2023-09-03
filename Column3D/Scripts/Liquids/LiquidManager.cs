using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiquidManager : MonoBehaviour
{
    public bool IsReverse = false; 

    private LiquidController[] liquids;
    private Coroutine currentRoutine;

    private void Awake()
    {
        liquids = GetComponentsInChildren<LiquidController>();

        if (IsReverse)
        {
            liquids = liquids.Reverse().ToArray();
        }
    }

    public IEnumerator StartFilling(int startIndex = 0)
    {
        if (liquids != null)
        {
            if (startIndex < liquids.Length && startIndex >= 0)
            {
                for (int i = startIndex; i < liquids.Length; i++)
                {
                    int currentIndex = i;
                    if (IsReverse)
                    {
                        liquids[currentIndex].StartFillingBoards(() => liquids[currentIndex].StartFillingPlate());
                        yield return new WaitWhile(() => !liquids[currentIndex].plateIsFull);
                    }
                    else
                    {
                        liquids[currentIndex].StartFillingPlate(() => liquids[currentIndex].StartFillingBoards());
                        yield return new WaitWhile(() => !liquids[currentIndex].boardsIsFull);
                    }
                }
            }
        }
        currentRoutine = null;
    }
    public IEnumerator StartDrainage(int startIndex = 0)
    {
        if (liquids != null)
        {
            if (startIndex < liquids.Length && startIndex >= 0)
            {
                for (int i = startIndex; i < liquids.Length; i++)
                {
                    int currentIndex = i;
                    if (IsReverse)
                    {
                        liquids[currentIndex].StartDrainagePlate(() => liquids[currentIndex].StartDrainageBoards());
                        yield return new WaitWhile(() => liquids[currentIndex].boardsIsFull);
                    }
                    else
                    {
                        liquids[currentIndex].StartDrainageBoards(() => liquids[currentIndex].StartDrainagePlate());
                        yield return new WaitWhile(() => liquids[currentIndex].plateIsFull);
                    }
                }
            }
        }
    }
    public void ShowPlates()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.ShowPlate();
        }

    }
    public void ShowBoards()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.ShowBoards();
        }
     }
    public void ShowPlatesNBoards()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.ShowPlate();
            liquid.ShowBoards();
        }
    }
    public void HidePlates()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.HidePlate();
        }
    }
    public void HideBoards()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.HideBoards();
        }
    }
    public void HidePlatesNBoards()
    {
        StopCurrentRoutine();

        foreach (var liquid in liquids)
        {
            liquid.HidePlate();
            liquid.HideBoards();
        }
    }
    public void ShowPlatesFrom(LiquidController liquid)
    {
        StopCurrentRoutine();

        if (liquids != null)
        {
            if (liquids.Contains(liquid))
            {
                int startIndex = Array.IndexOf(liquids, liquid);
                for (int i = startIndex; i < liquids.Length; i++)
                {
                    liquids[i].ShowPlate();
                }
            }
        }
    }
    public void ShowPlatesNBoardsFrom(LiquidController liquid)
    {
        StopCurrentRoutine();

        if (liquids != null)
        {
            if (liquids.Contains(liquid))
            {
                int startIndex = Array.IndexOf(liquids, liquid);
                for (int i = startIndex; i < liquids.Length; i++)
                {
                    liquids[i].ShowPlate();
                    liquids[i].ShowBoards();
                }
            }
        }
    }
    public void StartFillingFrom(LiquidController liquid)
    {
        if (liquids != null)
        {
            if (liquids.Contains(liquid))
            {
                int startIndex = Array.IndexOf(liquids, liquid);
                StopCurrentRoutine();

                currentRoutine = StartCoroutine(StartFilling(startIndex));
            }
        }
    }
    public void StartDrainageFrom(LiquidController liquid)
    {
        if (liquids != null)
        {
            if (liquids.Contains(liquid))
            {
                int startIndex = Array.IndexOf(liquids, liquid);
                StopCurrentRoutine();

                currentRoutine = StartCoroutine(StartDrainage(startIndex));
            }
        }
    }

    private void StopCurrentRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }
}
