using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectConfig", menuName = "Data/ProjectConfig")]
public partial class ProjectConfig : ScriptableObjectInstaller
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private TransitionWindow _transitionWindowPrefab;

    public override void InstallBindings()
    {
        BindConfigs();
        BindTransitionWindow();
        BindCursor();
    }

    private void BindConfigs()
    {
        Container.BindInstance(_playerData).AsSingle();
    }

    private void BindTransitionWindow()
    {
        Container.Bind<TransitionWindow>()
            .FromComponentInNewPrefab(_transitionWindowPrefab)
            .AsSingle();
    }

    private void BindCursor()
    {
        Container.Bind<CursorController>().AsSingle();
    }
}
