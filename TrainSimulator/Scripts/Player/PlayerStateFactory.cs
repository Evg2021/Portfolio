using ModestTree;

public enum PlayerStates
{
    Moving,
    Interacting
}

public class PlayerStateFactory
{
    readonly PlayerStateMoving.Factory _movingFactory;
    readonly PlayerStateInteracting.Factory _controlPanelFactory;

    public PlayerStateFactory(
        PlayerStateMoving.Factory movingFactory,
        PlayerStateInteracting.Factory controlPanelFactory)
    {
        _movingFactory = movingFactory;
        _controlPanelFactory = controlPanelFactory;
    }

    public PlayerState CreateState(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Moving:
                {
                    return _movingFactory.Create();
                }
            case PlayerStates.Interacting:
                {
                    return _controlPanelFactory.Create();
                }
        }

        throw Assert.CreateException();
    }
}
