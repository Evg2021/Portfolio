using System;

public abstract class PlayerState : IDisposable
{
    public virtual void Update()
    {
        // optionally overridden
    }

    public virtual void FixedUpdate()
    {
        // optionally overridden
    }

    public virtual void Start()
    {
        // optionally overridden
    }

    public virtual void Dispose()
    {
        // optionally overridden
    }

    public virtual void LateUpdate()
    {
        // optionally overridden
    }
}