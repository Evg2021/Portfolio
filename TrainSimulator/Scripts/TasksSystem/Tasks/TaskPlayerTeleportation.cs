using UnityEngine;
using Zenject;

public class TaskPlayerTeleportation : BaseTask
{
    [SerializeField] private Transform _toPostion;
    
    private Player _player;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = GetType().Name;
    }

    [Inject]
    private void Construct(Player player)
    {
        _player = player;
    }
    protected override void LaunchTask()
    {
        _player.transform.position = _toPostion.position;
        _player.transform.rotation = _toPostion.rotation;
        FireCompleteSignal();
    }

    public override void Dispose()
    {
    }
}