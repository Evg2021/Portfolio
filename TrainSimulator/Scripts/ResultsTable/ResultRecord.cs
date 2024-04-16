public class ResultRecord
{
    public string Name => _name;
    public string Description => _description;
    public TaskObservationDiary ObservationDiary => _observationDiary;

    private string _name;
    private string _description;
    private TaskObservationDiary _observationDiary;

    public ResultRecord(BaseTask task, TaskObservationDiary observationDiary)
    {
        _name = task.Description.Name;
        _description = task.Description.Content;
        _observationDiary = observationDiary;
    }
}
