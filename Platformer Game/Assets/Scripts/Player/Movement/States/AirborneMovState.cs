using UnityEngine;

public class AirborneMovState : MovementState
{
    public AirborneMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }
    public override void EnterState()
    {
        //Debug.Log("enter airborne");
    }

    public override void ExitState()
    {
        MovementContext.animator.SetBool("Grounded", true);
        //Debug.Log("exit grounded");
    }

    public override MovementStateMachine.MovementStates GetNextState()
    {
         RaycastHit hit;
        
        // mid down raycast
        Ray ray = new Ray(MovementContext.noor.transform.position + Vector3.up,Vector3.down);

        if(MovementContext.noor.GetVelocity().y > 2)
        {
            return StateKey;
        }

        if(Physics.Raycast(ray, out hit, 1.2f, MovementContext.noor.GetCollisionLayers()))
        {
            // ground
            return MovementStateMachine.MovementStates.Grounded;
        }

        return StateKey;
    }

    public override void UpdateState()
    {
        MovementContext.noor.SetVelocity(
                MovementContext.noor.GetVelocity() + Vector3.down * MovementContext.noor.GravityForce * Time.deltaTime
        );

        MovementContext.noor.transform.Translate(MovementContext.noor.GetVelocity() * Time.deltaTime, Space.World);
        MovementContext.animator.SetFloat("Y Speed", Mathf.Clamp(MovementContext.noor.GetVelocity().y/5,-1,1));
    }
}
