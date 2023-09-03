using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerController : SingletonMonoBehaviour<CameraFollowerController>
{
    public static Transform Follow { get; private set; }
    public static bool IsInitialized;

    public static CinemachineVirtualCamera cinemachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        IsInitialized = false;
        cinemachine = GetComponent<CinemachineVirtualCamera>();

        if (!cinemachine)
        {
            Debug.LogError($"CameraFollowerController is missing {nameof(CinemachineVirtualCamera)} component on {transform.name} object.");
        }
        else
        {
            IsInitialized = true;
        }
    }

    public static void SetFollowObject(Transform follow)
    {
        Follow = follow;

        if (cinemachine)
        {
            cinemachine.Follow = follow;
        }
    }
}
