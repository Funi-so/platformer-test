using UnityEngine;

public abstract class MovementState : BaseState<MovementStateMachine.MovementStates>
{
    public MovementState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }
}
