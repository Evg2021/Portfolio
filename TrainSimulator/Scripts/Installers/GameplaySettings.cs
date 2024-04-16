using System;

[Serializable]
public class GameplaySettings
{
    public CameraSettings Camera;
    public PostProcessingSwitchSettings PostProcessing;
    public OutlineSettings Outline;
    public PlayerSettings Player;

    public GameplaySettings Clone()
    {
        GameplaySettings gameplaySettings = new()
        {
            Camera = (CameraSettings)Camera.Clone(),
            PostProcessing = (PostProcessingSwitchSettings)PostProcessing.Clone(),
            Outline = (OutlineSettings)Outline.Clone(),
            Player = (PlayerSettings)Player.Clone()
        };

        return gameplaySettings;
    }
}