using UnityEngine;

public class HandHeldInteractable : Interactable
{
    public virtual void PlayAnimation()
    {
        Debug.Log($"{gameObject.name} is playing animation");
    }
}