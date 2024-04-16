using UnityEngine;

public class SetModeButton : SetPlayerDataValueButton
{
    [SerializeField] private GameplayMode _mode;

    public override void OnClick()
    {
        if (_handler == null) return;

        _handler.SetGameplayMode(_mode);
    }
}