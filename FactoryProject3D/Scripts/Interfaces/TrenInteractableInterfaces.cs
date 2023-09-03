public interface IInteractable
{
    public void EnableHighlight();
    public void DisableHighlight();
    public IController GetMainController();
    public IController GetSecondController();
}

public interface ITrenInteractableFloat : ITrenInteractable
{
    public float GetSimulatorValue();
    public float GetLocalValue();
    public void Interact(float value);
    public void ChangeSetter(float value);
}

public interface ITrenInteractableBool : ITrenInteractable
{
    public bool GetLocalValue();
    public bool GetSimulatorValue();
    public void Interact(bool value);
}

public interface ITrenInteractableInt : ITrenInteractable
{
    public int GetLocalValue();
    public int GetSimulatorValue();
    public void Interact(int value);
}

public interface ITrenInteractable
{
    public ObjectMode GetObjectMode();
    public bool IsObjectInitialized();
    public bool IsObjectRegistrated();
    public string GetTrenName();
    public uint GetTrenIndex();
}