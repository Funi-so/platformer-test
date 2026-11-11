using UnityEngine;

public class JumpingMovState : MovementState
{
    const float jumpsquat = 0.2f;
    float timer = 0;
    public JumpingMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }
    public override void EnterState()
    {
        timer = jumpsquat;
    }

    public override void ExitState()
    {
        
    }

    public override MovementStateMachine.MovementStates GetNextState()
    {
        if(timer > 0)
        {
            return StateKey;
        }
        else
        {
            timer = 0;
            MovementContext.noor.SetVelocity(MovementContext.noor.GetVelocity() + Vector3.up*5);
            
            return MovementStateMachine.MovementStates.Airborne;
        }

    }

    public override void UpdateState()
    {
        // drag
        MovementContext.noor.SetVelocity(
                    MovementContext.noor.GetVelocity().normalized 
                    * Mathf.Max(0, MovementContext.noor.GetVelocity().magnitude 
                    - MovementContext.noor.GroundStopping * Time.deltaTime)
            );
        
        MovementContext.noor.transform.Translate(MovementContext.noor.GetVelocity() * Time.deltaTime, Space.World);
        timer -= Time.deltaTime;
    }
}
