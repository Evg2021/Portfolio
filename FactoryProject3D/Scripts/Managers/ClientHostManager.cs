using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClientHostManager : SingletonMonoBehaviour<ClientHostManager>
{
    public NetworkObject EnvironmentSynchronizer;

    public event Action OnServerConnection;
    public event Action OnServerDisconnection;
    public event Action<ConnectionState> ConnectionStateChanged;

    private GameObject player;
    private NetworkManager networkManager;

    public const string nameFolder = "Screenshots";
    private int screenshotIndex = 0;

    public bool allowReconnection;
    public float reconnectionDelay = 5.0f;
    public float waitConnectionTime = 10.0f;

    [Header("Debug")]
    public bool DirtyHost;
    public bool DirtyClient;

    public bool isConnected;
    public bool IsConnected 
    { 
        get
        {
            return isConnected;
        }
        set
        {
            if (value != isConnected)
            {
                isConnected = value;
                if (!value)
                {
                    OnServerDisconnection?.Invoke();
                }
                else
                {
                    OnServerConnection?.Invoke();
                }
            }
        }
    }

    private Coroutine currentRoutine;
    private Coroutine connectionRoutine;

    private ConnectionState currentConnectionState;
    public ConnectionState CurrentConnectionState
    {
        get
        {
            return currentConnectionState;
        }
        private set
        {
            currentConnectionState = value;
            ConnectionStateChanged?.Invoke(value);
        }
    }

    public GameObject test;

    private void Start()
    {
        allowReconnection = true;

        InitializeNetworkManager();
        InitializeUIManager();
        StartHostClient();
    }

    private void ClientHostManager_OnServerConnection()
    {
        CurrentConnectionState = ConnectionState.CONNECTED;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (connectionRoutine != null)
        {
            StopCoroutine(connectionRoutine);
            connectionRoutine = null;
        }
    }

    private void ClientHostManager_OnServerDisconnection()
    {
        CurrentConnectionState = ConnectionState.DISCONNECTED;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator WaitConnection()
    {
        yield return new WaitForSeconds(waitConnectionTime);
        
        if (!IsConnected)
        {
            CurrentConnectionState = ConnectionState.CONNECTIONFAILED;
        }

        connectionRoutine = null;
    }

    private void Update()
    {
        if (networkManager)
        {
            IsConnected = networkManager.IsConnectedClient;
        }

        TakeScreenshots();
    }

    private void InitializeNetworkManager()
    {
        networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager is missing on scene.");
        }
        else
        {
            if (networkManager.TryGetComponent<UNetTransport>(out var transport))
            {
                transport.ConnectAddress = string.IsNullOrEmpty(Settings.IPAddress) ? GlobalVariables.LocalIPAdress : Settings.IPAddress;
#if UNITY_EDITOR
                transport.ConnectAddress = "192.168.111.220";
#endif
            }
        }
    }
    private void InitializeUIManager()
    {
        if (transform.parent)
        {
            var uiManager = transform.parent.GetComponentInChildren<ClientUIManager>();
            if (uiManager)
            {
                uiManager.Initialize();
            }
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

    private bool StartHostClient()
    {
        if (networkManager && !networkManager.IsConnectedClient)
        {
            if (DirtyHost)
            {
                return StartHost();
            }
            else if (DirtyClient)
            {
                return StartClient();
            }
            else
            {
                if (Settings.CurrentMultiplayerMode == MultiplayerMode.CLIENT)
                {
                    return StartClient();
                }

                if (Settings.CurrentMultiplayerMode == MultiplayerMode.HOST)
                {
                    return StartHost();
                }
            }
        }

        return false;
    }
    private bool StartClient()
    {
        CurrentConnectionState = ConnectionState.CONNECTING;

        OnServerConnection += ClientHostManager_OnServerConnection;
        OnServerDisconnection += ClientHostManager_OnServerDisconnection;

        connectionRoutine = StartCoroutine(WaitConnection());
        return networkManager.StartClient();
    }
    private bool StartHost()
    {
        networkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        bool result = networkManager.StartHost();

        InitializeEnvironmentSynchronizer();

        return result;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        Debug.Log($"Client {obj} disconnected.");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log($"Client {obj} connected.");
    }

    private void Disconnect()
    {
        if (networkManager)
        {
            networkManager.Shutdown();
        }
    }

    public static void RemoveNetworkManager()
    {
        if (Instance && Instance.networkManager)
        {
            Instance.networkManager.Shutdown();
            Destroy(Instance.networkManager.gameObject);
        }
    }
    public static void Restart()
    {
        RemoveNetworkManager();
        Utilities.RestartCurrentScene();
    }
    public static void BackToMenu()
    {
        RemoveNetworkManager();
        SceneManager.LoadScene(GlobalVariables.MenuSceneName);
    }

    private void TakeScreenshots()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            CheckScreenshotsDirectory();

            ScreenCapture.CaptureScreenshot($"{nameFolder}/{screenshotIndex + 1}.png");
        }
    }

    private void CheckScreenshotsDirectory()
    {
        if (Directory.Exists(nameFolder))
        {
            var files = Directory.GetFiles(nameFolder);
            if (files.Length == 0)
            {
                screenshotIndex = 0;
            }
            else
            {
                screenshotIndex = files.Length;
            }
        }
        else
        {
            Directory.CreateDirectory(nameFolder);
            screenshotIndex = 0;
        }
    }
}

public enum ConnectionState
{
    WAIT, CONNECTING, CONNECTED, CONNECTIONFAILED, DISCONNECTED
}