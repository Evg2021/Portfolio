using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SingletonNetworkBehaviour<T> : NetworkBehaviour
    where T : NetworkBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = Instance;
        }
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
