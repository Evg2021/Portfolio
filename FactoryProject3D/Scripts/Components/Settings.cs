using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Settings
{
    public static Resolution Resolution { get; private set; }
    public static bool IsFullScreen { get; private set; }
    public static string IPAddress { get; private set; }
    public static MultiplayerMode CurrentMultiplayerMode { get; private set; }

    private static string settingsFilename = "Settings.json";

    public static void SetResolution(Resolution newResolution)
    {
        Resolution = newResolution;
        Screen.SetResolution(Resolution.width, Resolution.height, Screen.fullScreen);
    }
    public static void SetFullScreen(bool fullScreen)
    {
        IsFullScreen = fullScreen;
        Screen.SetResolution(Screen.width, Screen.height, IsFullScreen);
    }
    public static void SetIPAddress(string address)
    {
        IPAddress = address;
    }
    public static void SetMultiplayerMode(MultiplayerMode mode)
    {
        CurrentMultiplayerMode = mode;
    }

    private static bool CheckSettingsFileExisting(out SystemSettings settings)
    {
        bool result = true;

        if (!Utilities.CheckStreamingAssetsPath())
        {
            result = false;
        }

        if (!File.Exists(Application.streamingAssetsPath + '\\' + settingsFilename))
        {
            using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + '\\' + settingsFilename))
            {
                settings = CreateDefaultSettings();
                ReadSettings(settings);
                writer.Write(JsonUtility.ToJson(settings));
            }

            result = false;
        }
        else
        {
            using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + '\\' + settingsFilename))
            {
                var data = reader.ReadToEnd();
                settings = JsonUtility.FromJson<SystemSettings>(data);
            }
        }


        return result;
    }
    public static void InitiailzeSettings()
    {
        if (CheckSettingsFileExisting(out var settings))
        {
            ReadSettings(settings);
        }
    }
    public static void SaveSettings()
    {
        if (File.Exists(Application.streamingAssetsPath + '\\' + settingsFilename))
        {
            using (StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + '\\' + settingsFilename))
            {
                var settings = new SystemSettings()
                {
                    ResolutionParameter = new Vector2(Resolution.width, Resolution.height),
                    IsFullScreenParameter = IsFullScreen,
                    IPAddressParameter = IPAddress
                };
                writer.Write(JsonUtility.ToJson(settings));
            }
        }
    }
    
    private static SystemSettings CreateDefaultSettings()
    {
        return new SystemSettings()
        {
            ResolutionParameter = new Vector2(Screen.width, Screen.height),
            IsFullScreenParameter = Screen.fullScreen,
            IPAddressParameter = "127.0.0.1"
        };
    }
    private static void ReadSettings(SystemSettings settings)
    {
        var newResolution = new Resolution()
        {
            width = (int)settings.ResolutionParameter.x,
            height = (int)settings.ResolutionParameter.y
        };

        SetResolution(newResolution);
        SetFullScreen(settings.IsFullScreenParameter);
        SetIPAddress(settings.IPAddressParameter);
    }
}

public enum MultiplayerMode
{
    SERVER, CLIENT, HOST
}

[Serializable]
public struct SystemSettings
{
    public Vector2 ResolutionParameter;
    public bool IsFullScreenParameter;
    public string IPAddressParameter;
}