using System.Collections.Generic;
using Zenject;
using IInitializable = Zenject.IInitializable;

public class ScenarioInitializator : IInitializable
{
    private PlayerData _playerData;
    private ScenarioHoldersList _scenarioHoldersList;
    private TaskController _taskController;
    private TimerDefault _timer;

    public ScenarioInitializator(PlayerData playerData, ScenarioHoldersList scenarioHoldersList, 
        TaskController taskController, [Inject(Id ="General")] TimerDefault timer)
    {
        _playerData = playerData;
        _scenarioHoldersList = scenarioHoldersList;
        _taskController = taskController;
        _timer = timer;
    }

    public void Initialize()
    {
        _taskController.Launch(GetTaskByRole());
        _timer.SetTimer();
    }

    private List<BaseTask> GetTaskByRole()
    {
        List<BaseTask> tasks = new();

        ScenarioHolder ScenarioHolder = _scenarioHoldersList.ActiveScenarioHolder;

        switch (_playerData.Role)
        {
            case GameplayRole.TrainDriver:
                tasks.AddRange(ScenarioHolder.TrainDriverTasks);
                break;

            case GameplayRole.Assistant:
                tasks.AddRange(ScenarioHolder.AssistantTasks);
                break;
        }

        return tasks;
    }
}