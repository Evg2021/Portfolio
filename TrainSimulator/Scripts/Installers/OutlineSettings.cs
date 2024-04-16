using System;
using UnityEngine;
using static Outline;

[Serializable]
public class OutlineSettings : InstallerSettings
{
    public Mode OutlineMode;
    public Color OutlineColor;
    public float OutlineWidth;
}