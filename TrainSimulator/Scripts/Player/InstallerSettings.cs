using System;

[Serializable]
public class InstallerSettings : ICloneable
{
    public virtual object Clone() => MemberwiseClone();
}