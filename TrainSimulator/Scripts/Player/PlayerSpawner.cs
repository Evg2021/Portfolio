using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform _driverSpawnPosition;
    [SerializeField] private Transform _assistantSpawnPosition;
    
    [Inject(Id = "driver")] private DriverInteract _driverModel;
    [Inject(Id = "assistant")] private DriverInteract _assistantModel;
    
    private Player _player;
    private PlayerData _playerData;

    [Inject]
    private void Construct(PlayerData playerData, Player player)
    {
        _player = player;
        _playerData = playerData;
        
        Transform position = _playerData.Role == GameplayRole.TrainDriver ? _driverSpawnPosition : _assistantSpawnPosition;
        
        _player.transform.position = position.transform.position;

        _driverModel.gameObject.SetActive(_playerData.Role == GameplayRole.Assistant);
        _assistantModel.gameObject.SetActive(_playerData.Role == GameplayRole.TrainDriver);
    }
}