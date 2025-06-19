public interface ICharacterState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine
{
    private ICharacterState _currentState;

    public void ChangeState(ICharacterState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update() => _currentState?.Update();
}