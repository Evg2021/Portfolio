using UnityEngine;
using NaughtyAttributes;
using Zenject;


public class TaskInsertChunk : TaskAction
{
    [SerializeField] private Chunk _chunk;
    [SerializeField] private ChunkType _insertType;

    [ShowIf("_insertType", ChunkType.Specific)]
    [SerializeField] private int _insertAtChunk;

    private Treadmill _treadmill;

    [Inject]
    private void Construct(Treadmill treadmill)
    {
        _treadmill = treadmill;
    }

    protected override void LaunchTask()
    {

        switch(_insertType)
        {
            case ChunkType.Specific:
                _treadmill.InsertChunk(_chunk, _insertAtChunk);
                break;

            case ChunkType.LastInTreadmill:
                _treadmill.InsertChunk(_chunk);
                break;
        }        

        base.LaunchTask(); 
    }

    public enum ChunkType
    {
        Specific,
        LastInTreadmill
    }
}
