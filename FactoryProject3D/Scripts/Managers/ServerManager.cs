using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : SingletonMonoBehaviour<ServerManager>
{
    public NetworkObject EnvironmentSynchronizer;

    private NetworkManager networkManager;
    private Dictionary<ulong, ClientHostManager> Clients;

    void Start()
    {
        InitializeNetworkManager();
        StartServer();
    }

    private void StartServer()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            networkManager.StartServer();

            InitializeEnvironmentSynchronizer();

            Debug.Log("Server started.");
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        
        Debug.Log($"Client {obj} disconnected.");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log($"Client {obj} connected.");
    }

    private void InitializeNetworkManager()
    {
        networkManager = NetworkManager.Singleton;
        if (networkManager != null)
        {
            Debug.Log("NetworkManager was Initialized.");
        }
        else
        {
            Debug.LogError("NetworkManager is missing on scene.");
        }
    }
    private void InitializeEnvironmentSynchronizer()
    {
        if (EnvironmentSynchronizer)
        {
            var syncher = Instantiate(EnvironmentSynchronizer);
            syncher.Spawn();
        }
    }

    private void NetworkManager_ConnectionApprovalCallback(byte[] arg1, ulong arg2, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool approve = true;
        bool createPlayerObject = true;

        callback(createPlayerObject, null, approve, Vector3.zero, Quaternion.identity);

        Debug.Log("Client connected.");
    }
}
