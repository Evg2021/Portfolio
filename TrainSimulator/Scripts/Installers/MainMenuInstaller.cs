using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallSignals();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
    }
}