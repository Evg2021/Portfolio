using System.Collections.Generic;
using UnityEngine;

public class CursorController
{
    private List<ICursorControllable> _users = new();

    public void AddUser(ICursorControllable user)
    {
        if (_users.Contains(user)) return;

        _users.Add(user);
        SetVisible(true);
    }

    public void RemoveUser(ICursorControllable user)
    {
        if (!_users.Contains(user)) return;

        _users.Remove(user);

        if (_users.Count == 0)
            SetVisible(false);
    }

    public void ClearUsers()
    {
        _users.Clear();
        SetVisible(false);
    }

    private void SetVisible(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }
}