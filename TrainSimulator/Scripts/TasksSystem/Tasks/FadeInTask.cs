using UnityEngine;
using Zenject;

public class FadeInTask : TaskAwaiting
{
    [Tooltip("��� �������� ���������, ����� ��������� ������ 'N' � ��� �����, ��� ����� ����������� �����")]
    [SerializeField] private string _messageText = "������ N �����";
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