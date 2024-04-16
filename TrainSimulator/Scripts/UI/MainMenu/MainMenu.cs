using UnityEngine;
using Zenject;

public class MainMenu : MonoBehaviour, ICursorControllable
{
    [Inject] private CursorController _curosrController;

    private void Start()
    {
        _curosrController.AddUser(this);
    }
}
