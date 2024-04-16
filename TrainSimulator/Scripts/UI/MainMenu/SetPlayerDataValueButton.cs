using UnityEngine;

public abstract class SetPlayerDataValueButton : MonoBehaviour
{
    [SerializeField] protected MainMenuPlayerDataHandler _handler;
    public abstract void OnClick();
}