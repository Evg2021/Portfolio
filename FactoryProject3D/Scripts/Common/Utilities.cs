using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public static void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static string GetStringFromIntPtr(IntPtr pointer, Encoding encoding)
    {
        var bytes = GetBytesFromIntPtr(pointer);
        if (bytes != null)
        {
            return encoding.GetString(bytes);
        }
        return string.Empty;
    }
    public static byte[] GetBytesFromIntPtr(IntPtr pointer)
    {
        int length = 0;
        while (Marshal.ReadByte(pointer, length) != 0) ++length;
        byte[] bytes = new byte[length];
        Marshal.Copy(pointer, bytes, 0, length);
        return bytes;
    }

    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public static bool CheckStreamingAssetsPath()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            return true;
        }
        else
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        return false;
    }

    public static GameObject GenerateOutline(MeshRenderer renderer, Material material)
    {
        var outline = new GameObject(renderer.name + "Outline");

        outline.transform.parent = renderer.transform;
        outline.transform.localScale = Vector3.one;
        outline.transform.localPosition = Vector3.zero;
        outline.transform.localRotation = Quaternion.identity;

        var meshFilter = outline.AddComponent<MeshFilter>();
        var mesh = renderer.GetComponent<MeshCollider>()?.sharedMesh;
        if (!mesh)
        {
            mesh = renderer.GetComponent<MeshFilter>().mesh;
        }
        meshFilter.mesh = mesh;
        var meshRenderer = outline.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        return outline;
    }
}
