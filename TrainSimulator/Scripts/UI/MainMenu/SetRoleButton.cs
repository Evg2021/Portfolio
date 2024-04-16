using UnityEngine;

public class SetRoleButton : SetPlayerDataValueButton
{
    [SerializeField] private GameplayRole _role;

    public override void OnClick()
    {
        if (_handler == null) return;

        _handler.SetGameplayRole(_role);
    }
}