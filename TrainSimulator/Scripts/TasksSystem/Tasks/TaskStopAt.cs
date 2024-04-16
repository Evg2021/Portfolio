using UnityEngine;
using NaughtyAttributes;
using Zenject;

public class TaskStopAt : TaskAction
{
    [SerializeField] private Vector3 _stopAt;
    [SerializeField] private ChunkType _stopType;

    [ShowIf("_stopType", ChunkType.Specific)]
    [SerializeField] private int _stopAtIndex;

    private Treadmill _treadmill;

    [Inject]
    private void Construct(Treadmill treadmill)
    {
        _treadmill = treadmill;
    }

    protected override void LaunchTask()
    {
        switch (_stopType)
        {
            case ChunkType.Specific:
                _treadmill.StopAtIndex(_stopAt, out _timerAwait, _stopAtIndex);
                break;

            case ChunkType.LastInTreadmill:
                _treadmill.StopAtLastChunk(_stopAt, out _timerAwait);
                break;

            case ChunkType.LastInserted:
                _treadmill.StopAtLastInsertedChunk(_stopAt, out _timerAwait);
                break;
        }

        base.LaunchTask();
    }

    public enum ChunkType
    {
        Specific,
        LastInTreadmill,
        LastInserted
    }
}