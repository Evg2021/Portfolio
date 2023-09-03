using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientUIManager : MonoBehaviour
{
    [Header("Connection Panel:")]

    public GameObject ConnectionPanel;
    public TextMeshProUGUI StatusText;
    public GameObject RestartButton;

    private ClientHostManager clientHostManager;

    private float connectingTimer;

    public void Initialize()
    {
        InitializeClientHostManager();
    }

    private void InitializeClientHostManager()
    {
        clientHostManager = ClientHostManager.Instance;
        if (!clientHostManager)
        {
            Debug.LogError("ClientHostManager was not initialized.");
        }
        else
        {
            clientHostManager.ConnectionStateChanged += ClientHostManager_ConnectionStateChanged;
        }
    }

    private void Update()
    {
        UpdateStatusTextWithTimer();
    }

    private void ClientHostManager_ConnectionStateChanged(ConnectionState state)
    {
        UpdateStatusText(state);

        if (ConnectionPanel)
        {
            if(state == ConnectionState.CONNECTING ||
               state == ConnectionState.CONNECTIONFAILED ||
               state == ConnectionState.DISCONNECTED)
            {
                ConnectionPanel.SetActive(true);
            }
            else
            {
                ConnectionPanel.SetActive(false);
            }
        }
    }
    private void UpdateStatusText(ConnectionState state)
    {
        if (StatusText)
        {
            switch (state)
            {
                case ConnectionState.CONNECTED:
                    StatusText.text = GlobalVariables.StatusConnected;

                    if (RestartButton && RestartButton.activeSelf)
                    {
                        RestartButton.SetActive(false);
                    }

                    break;

                case ConnectionState.CONNECTING:
                    connectingTimer = clientHostManager.waitConnectionTime;
                    StatusText.text = GlobalVariables.StatusConnecting + $" ({connectingTimer.ToString("00")} ñ)";

                    if(RestartButton && RestartButton.activeSelf)
                    {
                        RestartButton.SetActive(false);
                    }

                    break;

                case ConnectionState.CONNECTIONFAILED:
                    StatusText.text = GlobalVariables.StatusConnectionFailed;

                    if (RestartButton && !RestartButton.activeSelf)
                    {
                        RestartButton.SetActive(true);
                    }

                    break;

                case ConnectionState.DISCONNECTED:
                    StatusText.text = GlobalVariables.StatusDisconnected;

                    if (RestartButton && !RestartButton.activeSelf)
                    {
                        RestartButton.SetActive(true);
                    }

                    break;
            }
        }
    }
    private void UpdateStatusTextWithTimer()
    {
        if (StatusText && clientHostManager)
        {
            if (clientHostManager.CurrentConnectionState == ConnectionState.CONNECTING)
            {
                if (connectingTimer > 0)
                {
                    connectingTimer -= Time.deltaTime;
                }

                if (connectingTimer < 0)
                {
                    connectingTimer = 0.0f;
                }

                StatusText.text = GlobalVariables.StatusConnecting + $" ({connectingTimer.ToString("00")} ñ)";
            }
        }
    }

    public void OnClickRestart()
    {
        ClientHostManager.Restart();
    }
    public void OnClickBackToMenu()
    {
        ClientHostManager.BackToMenu();
    }
}
