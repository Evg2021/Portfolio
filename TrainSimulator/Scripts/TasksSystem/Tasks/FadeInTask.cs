using UnityEngine;
using Zenject;

public class FadeInTask : TaskAwaiting
{
    [Tooltip("При указании сообщения, нужно указывать символ 'N' в том месте, где будет указываться число")]
    [SerializeField] private string _messageText = "Прошло N минут";
    [SerializeField] private float _passedTime = 5f;

    private TimePassedFading _timePassedFading;

    [Inject]
    private void Construct(TimePassedFading timePassedFading)
    {
        _timePassedFading = timePassedFading;
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();

        string message = _messageText.Replace("N", _passedTime.ToString("#"));
        _timePassedFading.Show(message, _timerAwait);
    }
}