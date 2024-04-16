using UnityEngine;
using Zenject;

public class TaskQuestion : BaseTask
{
    [SerializeField] private Question _question;

    [Inject] private QuestionView _questionView;
    [Inject] private Player _player;
    [Inject] private InventoryPanel _inventoryPanel;

    protected override void LaunchTask()
    {
        _questionView.OnClickCorrectAnswer += FireCompleteSignal;
        _questionView.OnClickWrongAnswer += FireHardWrong;
        _questionView.Show(_question);
        _inventoryPanel.SelectDefaultSlot();
        _player.ChangeState(PlayerStates.Interacting);
        _player.SetLockStateSwitching(true);
    }

    public override void Dispose()
    {
        _questionView.OnClickCorrectAnswer -= FireCompleteSignal;
        _questionView.OnClickWrongAnswer -= FireHardWrong;
        _questionView.Hide();
        _player.SetLockStateSwitching(false);
    }
}
