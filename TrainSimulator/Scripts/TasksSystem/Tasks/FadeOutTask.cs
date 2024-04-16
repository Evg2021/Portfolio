using Zenject;

public class FadeOutTask : TaskAwaiting
{
    private TimePassedFading _timePassedFading;

    [Inject]
    private void Construct(TimePassedFading timePassedFading)
    {
        _timePassedFading = timePassedFading;
    }

    protected override void LaunchTask()
    {
        base.LaunchTask();

        _timePassedFading.Hide(_timerAwait);
    }
}
