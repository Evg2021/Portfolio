using UnityEngine;
using Zenject;

public class TaskAcceptableMessage : BaseTask
{
    [Inject] protected DescriptionAcceptView _descriptionAcceptView;

    protected override void LaunchTask()
    {
        _descriptionAcceptView.Show(_description);
        _descriptionAcceptView.OnClick += FireCompleteSignal;
    }

    public override void Dispose()
    {
        Unsubscribe();
        _descriptionAcceptView.OnClick -= FireCompleteSignal;
        _descriptionAcceptView.Hide();
    }

    protected override void OnStartInteract(InteractableSignal signal)
    {
        FireHardWrong();
    }
}