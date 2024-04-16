using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Data/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public GameplaySettings GameplaySettings;
    public DistanceParameters Distance;
    public MistakesChecker MistakesChecker;

    public override void InstallBindings()
    {
        InstallGameplaySettings();
        InstallPlayer();
    }

    private void InstallGameplaySettings()
    {
        Container.BindInstance(Distance);
        Container.BindInstance(GameplaySettings.Clone());
        Container.BindInstance(MistakesChecker).AsSingle();
    }

    private void InstallPlayer()
    {
        Container.Bind<PlayerStateFactory>().AsSingle();
        Container.BindFactory<PlayerStateMoving, PlayerStateMoving.Factory>().WhenInjectedInto<PlayerStateFactory>();
        Container.BindFactory<PlayerStateInteracting, PlayerStateInteracting.Factory>().WhenInjectedInto<PlayerStateFactory>();
    }    
}
