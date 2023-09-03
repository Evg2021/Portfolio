using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField]
    private float updatingTime = 0.5f;

    private List<uint> trenObjectsToUpdate;
    private Coroutine currentRoutine;
    public bool allowUpdating = true;
    public NetworkObject netObject;

    private void Awake()
    {
        if (transform.parent)
        {
            netObject = transform.parent.GetComponent<NetworkObject>();
        }
        trenObjectsToUpdate = new List<uint>();
    }

    private void Start()
    {
        StartUpdating();
    }

    private void Update()
    {
        if (netObject && netObject.IsOwner)
        {
            if (allowUpdating && currentRoutine == null)
            {
                StartUpdating();
            }

            if (!allowUpdating && currentRoutine != null)
            {
                StopUpdating();
            }
        }
        else
        {
            enabled = false;
        }
    }

    private void StartUpdating()
    {
        StopUpdating();

        currentRoutine = StartCoroutine(UpdateObjects());
    }

    private void StopUpdating()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    private IEnumerator UpdateObjects()
    {
        while (true)
        {
            if (trenObjectsToUpdate != null && trenObjectsToUpdate.Count > 0)
            {
                ClientSocketManager.UpdateGet(trenObjectsToUpdate.ToArray(), (uint)trenObjectsToUpdate.Count);
            }

            yield return new WaitForSecondsRealtime(updatingTime);
        }
    }

    private void AddTrenObject(uint index)
    {
        if (trenObjectsToUpdate != null && index != uint.MaxValue && !trenObjectsToUpdate.Contains(index))
        {
            trenObjectsToUpdate.Add(index);
        }
    }
    private void RemoveTrenObject(uint index)
    {
        if (trenObjectsToUpdate != null && trenObjectsToUpdate.Contains(index))
        {
            trenObjectsToUpdate.Remove(index);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var trenObjects = other.GetComponents<TrenObject>();

        if (trenObjects != null)
        {
            foreach (var trenObject in trenObjects)
            {
                AddTrenObject(trenObject.Index);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var trenObjects = other.GetComponents<TrenObject>();

        if (trenObjects != null)
        {
            foreach (var trenObject in trenObjects)
            {
                RemoveTrenObject(trenObject.Index);
            }
        }
    }
}
