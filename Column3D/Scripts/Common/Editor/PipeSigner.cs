using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PipeSigner : Editor
{
    [MenuItem("Tools/Sign Pipes")]
    private static void OpenMenu()
    {
        var pipes = GetPipeKeyObjects();
        foreach (var pipe in pipes)
        {
            if (pipe.TryGetComponent<Collider>(out _))
            {
                pipe.UpdateStreamsData();
            }
            else
            {
                pipe.RemoveStreamsData();
            }
        }
    }

    private static PipeKeyObject[] GetPipeKeyObjects()
    {
        var pipes = FindObjectsOfType<PipeKeyObject>().Where(h => h.transform.parent != null && h.transform.parent.parent == null);
        return pipes.ToArray();
    }
}
