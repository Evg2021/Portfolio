using System;

[Serializable]
public class PlayerSettings : InstallerSettings
{
    public PlayerBody.Settings Body;
    public PlayerStateMoving.Settings StateMoving;
    public PlayerStateInteracting.Settings StateInteracting;

    public override object Clone()
    {
        PlayerSettings playerSettings = new()
        {
            StateMoving = (PlayerStateMoving.Settings)StateMoving.Clone(),
            StateInteracting = (PlayerStateInteracting.Settings)StateInteracting.Clone(),
            Body = (PlayerBody.Settings)Body.Clone()
        };

        return playerSettings;
    }
}
