using System.Collections.Generic;
using UnityEngine;

public class ScenarioHolder : MonoBehaviour
{
    public SituationData SituationData => _situationData;

    [SerializeField] private SituationData _situationData;

    [SerializeField] private Transform _trainDriverTasksContainer;
    [SerializeField] private Transform _assistantTasksTasksContainer;

    public IReadOnlyList<BaseTask> TrainDriverTasks => _trainDriverTasks;
    [SerializeField] private List<BaseTask> _trainDriverTasks = new();
    public IReadOnlyList<BaseTask> AssistantTasks => _assistantTasks;
    [SerializeField] private List<BaseTask> _assistantTasks = new();

    private void OnValidate()
    {
        if (_situationData == null)
            gameObject.name = $"{_situationData.name} ({_situationData.Name})";

        AddChildsToList(_trainDriverTasksContainer, _trainDriverTasks);
        AddChildsToList(_assistantTasksTasksContainer, _assistantTasks);
    }

    private void AddChildsToList(Transform container, List<BaseTask> list)
    {
        list.Clear();

        if (container == null)
        {
            Debug.Log("Container with tasks is empty");
            return;
        }

        foreach (BaseTask task in container.GetComponentsInChildren<BaseTask>())
        {
            if (task.gameObject.activeSelf)
                list.Add(task);
        }
    }
}