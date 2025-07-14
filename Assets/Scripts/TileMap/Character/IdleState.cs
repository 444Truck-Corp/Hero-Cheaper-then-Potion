public class IdleState : ICharacterState
{
    private readonly CharacterMovement _character;
    private readonly StateMachine _machine;

    public IdleState(CharacterMovement character, StateMachine machine)
    {
        _character = character;
        _machine = machine;
    }

    public void Enter()
    {
        _character.Animator.SetPlaying(false);
    }

    public void Update()
    {
        _character.UpdateAnimation();

        if (_character.HasCommands)
        {
            _machine.ChangeState(new MoveState(_character, _machine));
        }
        else if (_character.CanAutoFind)
        {
            _machine.ChangeState(new AutoFindState(_character, _machine));
        }
    }

    public void Exit() { }
}

public class MoveState : ICharacterState
{
    private readonly CharacterMovement _character;
    private readonly StateMachine _machine;

    public MoveState(CharacterMovement character, StateMachine machine)
    {
        _character = character;
        _machine = machine;
    }

    public void Enter()
    {
        if (_character.HasCommands)
        {
            _character.StartMove();
        }
        else
        {
            _machine.ChangeState(new IdleState(_character, _machine));
        }
    }

    public void Update()
    {
        _character.UpdateAnimation();

        if (_character.MoveStep())
        {
            if (_character.HasCommands)
            {
                Enter();
            }
            else
            {
                _character.CompleteMove();
                _machine.ChangeState(new IdleState(_character, _machine));
            }
        }
    }

    public void Exit() { }
}

public class AutoFindState : ICharacterState
{
    private readonly CharacterMovement _character;
    private readonly StateMachine _machine;

    public AutoFindState(CharacterMovement character, StateMachine machine)
    {
        _character = character;
        _machine = machine;
    }

    public void Enter()
    {
        _character.Animator.SetPlaying(false);
    }

    public void Update()
    {
        _character.UpdateAnimation();

        if (!_character.WaitTimeElapsed()) return;

        _character.FindNewTarget();
        if (_character.HasCommands)
        {
            _machine.ChangeState(new MoveState(_character, _machine));
        }
        else
        {
            _machine.ChangeState(new IdleState(_character, _machine));
        }
    }

    public void Exit() { }
}