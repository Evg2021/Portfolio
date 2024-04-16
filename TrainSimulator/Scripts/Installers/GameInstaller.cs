using UnityEngine;
using Zenject;

public partial class GameInstaller : MonoInstaller
{
    

    public override void InstallBindings()
    {
        InstallSignals();
        InstallInput();
        InstallTimers();
        InstallSystems();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<PlayerStateChangeSignal>();

        Container.DeclareSignal<EnterPointerEntitySignal>();
        Container.DeclareSignal<ExitPointerEntitySignal>();

        Container.DeclareSignal<StartInteractSignal>();
        Container.DeclareSignal<MoveInteractSignal>();
        Container.DeclareSignal<StopInteractSignal>();

        Container.DeclareSignal<StartZoomAtSignal>();
        Container.DeclareSignal<StopZoomAtSignal>();

        Container.DeclareSignal<TaskStartedSignal>();
        Container.DeclareSignal<TaskEndedSignal>();
        Container.DeclareSignal<TaskWrongSignal>();
        Container.DeclareSignal<InstantlyStopScenario>();

        Container.DeclareSignal<ClickToolSlotSignal>();

        Container.DeclareSignal<OffsetIndicatorChangeConditionSignal>();

        Container.DeclareSignal<GamePauseSignal>();

        Container.DeclareSignal<PressureChangesSignal>();
    }
    private void InstallInput()
    {
        Container.Bind<PlayerInput>().AsSingle();
    }

    private void InstallSystems()
    {
        
        Container.BindInterfacesAndSelfTo<TaskController>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScenarioInitializator>().AsSingle();
        Container.BindInterfacesAndSelfTo<HiglightHolder>().AsSingle();
        Container.BindInterfacesAndSelfTo<ResultsRecorder>().AsSingle();
    }

    private void InstallTimers()
    {
        var viewGeneral = Container.ResolveId<TimerView>("General");
        var viewOptional = Container.ResolveId<TimerView>("Optional");

        GameObject newGameObject = new("EmptyTimerView");
        var emptyView = newGameObject.AddComponent<TimerView>();

        Container.Bind<TimerDefault>().WithId("General").AsCached().WithArguments(viewGeneral);
        Container.Bind<TimerReverse>().WithId("Optional").AsCached().WithArguments(viewOptional);
        Container.Bind<TimerDefault>().WithId("Empty").AsCached().WithArguments(emptyView);
    }
}

//PLAYER STATES
public class PlayerStateChangeSignal { public PlayerStates Value; }

//INTERACT STATE
public class EnterPointerEntitySignal { public RaycastEntity Value; }
public class ExitPointerEntitySignal { }

//INTERACTABLE
public class InteractableSignal { public RaycastEntity Value; }
public class StartInteractSignal : InteractableSignal { }
public class MoveInteractSignal : InteractableSignal { }
public class StopInteractSignal : InteractableSignal { }

//ZOOM AT
public class ZoomAtSignal { public RaycastEntity Value; }
public class StartZoomAtSignal : ZoomAtSignal { }
public class StopZoomAtSignal : ZoomAtSignal { }

//SCENARIO
public class TaskSignal { public BaseTask Task; }
public class TaskStartedSignal : TaskSignal { }
public class TaskEndedSignal : TaskSignal { }
public class TaskWrongSignal { public WrongType Type; }
public class InstantlyStopScenario { public FatalMistake Mistake; }

//INVENTORY
public class ClickToolSlotSignal { public ISlotComponentContainer Value; }

//OFFSCREEN ARROW
public class OffsetIndicatorChangeConditionSignal
{
    public OffscreenTarget OffscreenTarget;
    public bool Active;
}

//GAME STATES
public class GamePauseSignal { public bool Value; }

//PRESSURE CHANGES
public class PressureChangesSignal
{
    public float ToValue;
    public float Duration;
    public EPressureGaugeSide PressureGaugeSide;
    public EGaugeArrow Arrow;
}
