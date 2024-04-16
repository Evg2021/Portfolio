using UnityEngine;

public abstract class Door : Interactable, IItemInteractable
{
    [Space(10)]
    [Header("Door settings")]
    [SerializeField] protected Transform _door;

    public abstract void Open();
    public abstract void CloseDoor();
    public void InteractWithItem()
    {
        Open();
    }
}
