using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalVariables", menuName = "Create Global Variables Asset")]
public class GlobalVariables : SingletonScriptableObject<GlobalVariables>
{
    public const string LocalIPAdress = "127.0.0.1";

    public static string ServerSceneName        = "ServerScene";
    public static string ClientHostSceneName    = "MainScene";
    public static string MenuSceneName          = "MenuScene";

    public static string StatusConnecting       = "Подключение...";
    public static string StatusConnected        = "Подключен.";
    public static string StatusConnectionFailed = "Ошибка подключения.";
    public static string StatusDisconnected     = "Отключен.";
    public static string StatusWaiting          = "В ожидании.";

    public static string AimObjectName          = "Aim";
    public static string MainInfoObjectName     = "MainInfo";
    public static string SecondInfoObjectName   = "SecondInfo";

    [SerializeField]
    private Material primitiveHighlightMaterial;
    public static Material PrimitiveHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.primitiveHighlightMaterial;
            }
            return null;
        }
    }

    [SerializeField]
    private Material interactableObjectHighlightMaterial;
    public static Material InteractableObjectHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.interactableObjectHighlightMaterial;
            }
            return null;
        }
    }

    [SerializeField]
    private Material wrongBindHighlightMaterial;
    public static Material WrongBindHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.wrongBindHighlightMaterial;
            }

            return null;
        }
    }

    [SerializeField]
    private Material rightHighlightMaterial;
    public static Material RightHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.rightHighlightMaterial;
            }

            return null;
        }
    }

    [SerializeField]
    private Material outlineHighlightMaterial;
    public static Material OutlineHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.outlineHighlightMaterial;
            }

            return null;
        }
    }

    [SerializeField]
    private Material environmentOutlineHighlightMaterial;
    public static Material EnvironmentOutlineHighlightMaterial
    {
        get
        {
            if (Instance)
            {
                return Instance.environmentOutlineHighlightMaterial;
            }

            return null;
        }
    }
}
