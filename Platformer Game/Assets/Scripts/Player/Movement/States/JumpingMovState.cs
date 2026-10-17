using UnityEngine;

public class JumpingMovState : MovementState
{public 
    JumpingMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }
    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override MovementStateMachine.MovementStates GetNextState()
    {
        return StateKey;
    }

    public override void UpdateState()
    {
        
    }
}
