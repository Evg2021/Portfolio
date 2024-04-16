using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TaskController : IDisposable
{
    public delegate void GameComplete();
    public event GameComplete OnGameComplete = delegate { };

    private readonly SignalBus _signalBus;
    private readonly HiglightHolder _higlightHolder;
    private readonly TransitionWindow _sceneSwitch;
    private readonly PlayerData _playerData;
    
    private  List<BaseTask> _tasks;
    private BaseTask _currentTask;
    private int _taskIndex = 0;
    private MistakesChecker _mistakesChecker;
    private TimePassedFading _timePassedFading;

    public TaskController(SignalBus signalBus, HiglightHolder higlightHolder,
        TransitionWindow sceneSwitch, PlayerData playerData, MistakesChecker mistakesChecker, TimePassedFading timePassedFading)
    {
        _timePassedFading = timePassedFading;
        _mistakesChecker = mistakesChecker;
        _playerData = playerData;
        _higlightHolder = higlightHolder;
        _signalBus = signalBus;
        _sceneSwitch = sceneSwitch;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<TaskEndedSignal>(OnTaskEnded);
        _signalBus.Unsubscribe<TaskWrongSignal>(OnWrongAction);
        _signalBus.Unsubscribe<InstantlyStopScenario>(InstantlyStopScenario);
    }

    public void Launch(List<BaseTask> tasks)
    {
        _mistakesChecker.Reset();
            
        _tasks = tasks;

        if (_tasks.Count == 0)
        {
            Debug.LogError("No tasks added to the list!!!");
            return;
        }

        NextTask();

        _signalBus.Subscribe<TaskEndedSignal>(OnTaskEnded);
        _signalBus.Subscribe<TaskWrongSignal>(OnWrongAction);
        _signalBus.Subscribe<InstantlyStopScenario>(InstantlyStopScenario);
    }
    
    protected virtual void NextTask()
    {
        _currentTask = _tasks[_taskIndex];

        if (_currentTask != null)
            _currentTask.Initialize(_signalBus, _higlightHolder, _playerData);
    }

    protected virtual void OnTaskEnded(TaskEndedSignal obj)
    {
        _currentTask.Dispose();
        _taskIndex++;

        if (_taskIndex == _tasks.Count)
            CompleteGame();
        else
            NextTask();
    }

    protected virtual void CompleteGame()
    {
        OnGameComplete?.Invoke();
        _sceneSwitch.LoadScene("MainMenu", true);
    }

    private void OnWrongAction(TaskWrongSignal obj)
    {
        if (_playerData.Mode == GameplayMode.Exam)
        {
            bool allTrySpent = _mistakesChecker.AddMistakeAndCheck();
            if (allTrySpent)
                FatalErrorAndStopScenario();
        }
    }
    
    private void InstantlyStopScenario(InstantlyStopScenario signal)
    {
        _timePassedFading.Show(signal.Mistake.Text, 2);
        signal.Mistake.Task.StartCoroutine(InstantlyRestartScenario());
    }
    
    private IEnumerator InstantlyRestartScenario()
    {
        Debug.Log("FATAL ERROR!");

        _currentTask.Dispose();

        yield return new WaitForSeconds(4);

        _sceneSwitch.LoadScene("GameScene", true);
    }
    
    private void FatalErrorAndStopScenario()
    {
        Debug.Log("FATAL ERROR!");

        _currentTask.Dispose();
        _sceneSwitch.LoadScene("MainMenu", true);
    }
}