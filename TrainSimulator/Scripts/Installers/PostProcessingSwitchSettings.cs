using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

[Serializable]
public class PostProcessingSwitchSettings : InstallerSettings
{
    public float ChangeVolumeDuration;
    public List<Effect> Effects;

    [Serializable]
    public class Effect
    {
        public VolumeProfile Profile;
        public PlayerStates AssociatedState;
    }
}
