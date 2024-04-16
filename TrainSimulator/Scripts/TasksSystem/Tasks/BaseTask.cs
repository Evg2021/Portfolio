using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class BaseTask : MonoBehaviour
{
    public InfoMessage Description => _description;

    [SerializeField] protected InfoMessage _description;
    [SerializeField] protected InfoMessage _wrongMessage;
    [SerializeField] protected List<string> _softWrongMessages;

    [Inject] protected DescriptionView _descriptionView;
    [Inject] protected HardWrongMessageView _hardWrongMessageView;
    [Inject] protected SoftWrongMessageView _softWrongMessageView;

    protected SignalBus _signalBus;
    protected HiglightHolder _higlightHolder;
    protected PlayerData _playerData;

    protected virtual void OnValidate()
    {
        if (_description != null)
            gameObject.name = _description.Name + $"_{GetType().Name}";
    }

    public void Initialize(SignalBus signalBus, HiglightHolder higlightHolder, PlayerData playerData)
    {
        _playerData = playerData;
        _higlightHolder = higlightHolder;
        _signalBus = signalBus;

        Subscribe();
        FireStartSignal();
        LaunchTask();
    }

    public virtual void HiglightEnable()
    {
    }

    public virtual void HiglightDisable()
    {
    }


    protected virtual void LaunchTask()
    {
        print(_description.Name + " " + _description.Content);
        if (_playerData.Mode == GameplayMode.Training)
        {
            _descriptionView.Show(_description);
            HiglightEnable();
        }
    }

    public virtual void Dispose()
    {
        Unsubscribe();
        HiglightDisable();
        _descriptionView.Hide();
    }

    protected virtual void Subscribe()
    {
        _signalBus.Subscribe<StartInteractSignal>(OnStartInteract);
        _signalBus.Subscribe<StopInteractSignal>(OnStopInterract);
    }

    protected virtual void Unsubscribe()
    {
        _signalBus.TryUnsubscribe<StartInteractSignal>(OnStartInteract);
        _signalBus.TryUnsubscribe<StopInteractSignal>(OnStopInterract);
    }

    protected virtual void OnStopInterract(InteractableSignal signal)
    {
    }

    protected virtual void OnStartInteract(InteractableSignal signal)
    {
    }

    protected void ShowDescription()
    {
        _descriptionView.Show(_description);
    }

    protected void FireStartSignal()
    {
        TaskStartedSignal signal = new() { Task = this };
        _signalBus.Fire(signal);
    }

    protected void FireCompleteSignal()
    {
        TaskEndedSignal signal = new() { Task = this };
        _signalBus.Fire(signal);
    }

    protected void FireHardWrong()
    {
        _hardWrongMessageView.Show(_wrongMessage);
        _signalBus.Fire(new TaskWrongSignal() { Type = WrongType.Hard });

        Debug.Log($"Wrong signal from {gameObject.name}");
    }

    protected void FireSoftWrong(string message)
    {
        _softWrongMessageView.Show(message);
        _signalBus.Fire(new TaskWrongSignal() { Type = WrongType.Soft });
    }
}