using System.Collections.Generic;
using Zenject;
using System;

public class ResultsRecorder : IInitializable, IDisposable
{
    private string _scenarioName;
    private GameplayRole _role;
    private GameplayMode _mode;
    private string _fullName;
    private string _group;
    private DateTime _startTime;
    private List<ResultRecord> _results;
    private TaskObservationDiary _observationDiary;

    private SignalBus _signalBus;
    private TaskController _taskController;
    private TimerDefault _timer;
    private PlayerData _playerData;

    public ResultsRecorder(SignalBus signalBus, TaskController taskController, 
        [Inject(Id = "Empty")] TimerDefault timer, PlayerData playerData)
    {
        _signalBus = signalBus;
        _taskController = taskController;
        _timer = timer;
        _playerData = playerData;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<TaskWrongSignal>(HandleWrongSignal);
        _signalBus.Subscribe<TaskEndedSignal>(CreateResultRecord);
        _taskController.OnGameComplete += CreateResultsTable;

        Create(_playerData);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<TaskWrongSignal>(HandleWrongSignal);
        _signalBus.Unsubscribe<TaskEndedSignal>(CreateResultRecord);
        _taskController.OnGameComplete -= CreateResultsTable;
    }

    private void Create(PlayerData playerData)
    {
        _scenarioName = playerData.SituationData.Name;
        _role = playerData.Role;
        _mode = playerData.Mode;
        _fullName = playerData.Name;
        _group = playerData.Group;
        _startTime = DateTime.Now;

        _results = new();
        _observationDiary = new(_timer.TimePassed);
    }

    private void HandleWrongSignal(TaskWrongSignal signal)
    {
        WrongType type = signal.Type;

        if (type.Equals(WrongType.None)) return;

        _observationDiary.Wrongs[type].Add();

        UnityEngine.Debug.Log($"Result Recorder: Handle wrong signal. " +
            $"Wrongs count on current task: {_observationDiary.Wrongs[type].Count}");
    }

    private void CreateResultRecord(TaskEndedSignal signal)
    {
        BaseTask task = signal.Task;

        if (TaskIsValid(task))
            _results.Add(new ResultRecord(task, _observationDiary));

        _observationDiary = new(_timer.TimePassed);
    }

    private bool TaskIsValid(BaseTask task)
    {
        static bool IsEmpty(BaseTask task) => (task.Description.Name.Length + task.Description.Content.Length == 0);
        static bool IsSingleFrame(BaseTask task) => task is TaskAwaiting awaiting && awaiting.TimerAwait < 0.25f;
        static bool IsTaskAcceptableMessage(BaseTask task) => task is TaskAcceptableMessage;
        static bool IsTaskChangeGameSettings(BaseTask task) => task is TaskChangeGameSettings;
        static bool IsTaskFade(BaseTask task) => task is FadeInTask || task is FadeOutTask;

        return !IsEmpty(task)
            && !IsSingleFrame(task)
            && !IsTaskAcceptableMessage(task)
            && !IsTaskChangeGameSettings(task)
            && !IsTaskFade(task);
    }

    private void CreateResultsTable()
    {
        ResultsTableGenerator resultsTable = new();
        resultsTable.Create(_scenarioName, _role, _mode, _fullName, _group, _startTime, _timer.TimePassed, _results);
    }
}

public class TaskObservationDiary
{
    public IReadOnlyDictionary<WrongType, WrongRecord> Wrongs => _wrongs;

    public Dictionary<WrongType, WrongRecord> _wrongs = new ()
    {
        { WrongType.None, new WrongRecord() },
        { WrongType.Soft, new WrongRecord() },
        { WrongType.Hard, new WrongRecord() }
    };

    public TimeSpan StartTime => _startTime;
    private TimeSpan _startTime;

    public TaskObservationDiary(TimeSpan taskStartTime)
    {
        _startTime = taskStartTime;
    }
}

public class WrongRecord
{
    public int Count => _count;
    private int _count = 0;

    public void Add() => _count++;
}