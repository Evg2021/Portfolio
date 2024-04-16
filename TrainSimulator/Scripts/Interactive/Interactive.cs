using UnityEngine;

public class Interactive : MonoBehaviour
{
    public virtual void OnInteractStart(object value) {}
    public virtual void OnInteractTick(object value) {}
    public virtual void OnInteractEnd(object value) {}
}