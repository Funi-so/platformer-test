using UnityEngine;

public class JumpingMovState : MovementState
{
    const float jumpsquat = 0.4f;
    float timer = 0;
    public JumpingMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }
    public override void EnterState()
    {
        MovementContext.jumpBuffer = false;
        timer = jumpsquat;
        MovementContext.animator.SetBool("Grounded", false);
        MovementContext.animator.SetTrigger("Jump");
        MovementContext.animator.SetFloat("Y Speed", 1);
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
            if(MovementContext.noor.GetVelocity().magnitude < MovementContext.noor.MaxWalkSpeed){
                MovementContext.noor.SetVelocity(MovementContext.noor.GetVelocity().normalized * MovementContext.noor.MaxWalkSpeed*0.5f
                        + Vector3.up*6);
            }
            else
            {
                MovementContext.noor.SetVelocity(MovementContext.noor.GetVelocity().normalized * MovementContext.noor.MaxRunSpeed * 1.5f
                        + Vector3.up*4);
            }
            return MovementStateMachine.MovementStates.Airborne;
        }

    }

    public override void UpdateState()
    {
        // drag
        /*MovementContext.noor.SetVelocity(
                    MovementContext.noor.GetVelocity().normalized 
                    * Mathf.Max(0, MovementContext.noor.GetVelocity().magnitude 
                    - MovementContext.noor.GroundStopping * Time.deltaTime)
            );
        */
        MovementContext.noor.transform.Translate(MovementContext.noor.GetVelocity() * Time.deltaTime, Space.World);
        timer -= Time.deltaTime;
    }
}
