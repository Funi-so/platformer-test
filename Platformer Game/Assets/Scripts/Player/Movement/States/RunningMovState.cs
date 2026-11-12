using UnityEngine;

public class RunningMovState : MovementState
{
    public RunningMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }

    public override void EnterState()
    {
        //Debug.Log("enter running");
        RaycastHit hit;

        // mid down raycast
        Ray ray = new Ray(MovementContext.noor.transform.position + Vector3.up,Vector3.down);
        if(Physics.Raycast(ray, out hit, 1.2f, MovementContext.noor.GetCollisionLayers()))
        {
            // update ground normal
            MovementContext.groundNormal = hit.normal;
            // ground is sticky
            MovementContext.noor.transform.position = new Vector3(MovementContext.noor.transform.position.x,
                    Mathf.Max(hit.point.y,MovementContext.noor.transform.position.y),
                    MovementContext.noor.transform.position.z);
            
            MovementContext.noor.SetVelocity(Vector3.ProjectOnPlane(MovementContext.noor.GetVelocity(),hit.normal));
            Debug.Log("running started " + MovementContext.noor.GetVelocity());
        }

        // TODO "soft landing" logic
    }

    public override void ExitState()
    {
        //Debug.Log("exit running");
    }

    public override MovementStateMachine.MovementStates GetNextState()
    {
        RaycastHit hit;
        if(MovementContext.jumpBuffer)
            return MovementStateMachine.MovementStates.Jumping;
        // mid down raycast
        Ray ray = new Ray(MovementContext.noor.transform.position + Vector3.up,Vector3.down);
        if(Physics.Raycast(ray, out hit, 1.2f, MovementContext.noor.GetCollisionLayers()))
        {
            // back to walking
            if(MovementContext.noor.GetVelocity().magnitude < MovementContext.noor.MaxWalkSpeed)
            {
                return MovementStateMachine.MovementStates.Grounded;
            }

            // update ground normal
            MovementContext.groundNormal = hit.normal;
            // ground is sticky
            MovementContext.noor.transform.position = new Vector3(MovementContext.noor.transform.position.x,
                    Mathf.Max(hit.point.y,MovementContext.noor.transform.position.y),
                    MovementContext.noor.transform.position.z);
            
            // TODO "slide off" logic
        }
        else
        {
            // didn't find ground
            return MovementStateMachine.MovementStates.Airborne;
        }
        return StateKey;
    }

    public override void UpdateState()
    {
        if(MovementContext.inputVector.sqrMagnitude <= 0.001f)
        {
              MovementContext.noor.SetVelocity(
                    MovementContext.noor.GetVelocity().normalized 
                    * Mathf.Max(0, MovementContext.noor.GetVelocity().magnitude 
                    - MovementContext.noor.GroundStopping * Time.deltaTime)
            );
        }

        Vector3 forward = MovementContext.camera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        float currVel = MovementContext.noor.GetVelocity().magnitude;

        Vector3 worldInput = (MovementContext.camera.transform.right * MovementContext.inputVector.x) +
                (forward * MovementContext.inputVector.y);
        
        worldInput = Quaternion.FromToRotation(Vector3.up, MovementContext.groundNormal) * worldInput;

        // direction change
        if(MovementContext.inputVector.magnitude > 0)
        {
            MovementContext.noor.transform.rotation = Quaternion.RotateTowards(
                    MovementContext.noor.transform.rotation,
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(worldInput,Vector3.up),Vector3.up),
                    MovementContext.noor.RotationSpeed * Time.deltaTime
            );
        }

        Debug.Log("vel input dot " + Vector3.Dot(MovementContext.noor.GetVelocity(), worldInput));

        MovementContext.animator.SetBool("Grounded", true);
        // velocity change
        currVel += MovementContext.noor.GroundAcceleration*0.5f * Vector3.Dot(MovementContext.noor.GetVelocity(), worldInput) * Time.deltaTime;

        // velocity set
        MovementContext.noor.SetVelocity(
                MovementContext.noor.transform.forward * (currVel)
        );
        
        if(currVel >= MovementContext.noor.MaxRunSpeed)
        {
            MovementContext.noor.SetVelocity(MovementContext.noor.transform.forward * MovementContext.noor.MaxRunSpeed);
        }

        MovementContext.noor.transform.Translate(MovementContext.noor.GetVelocity() * Time.deltaTime, Space.World);
    }
}
