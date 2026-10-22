using UnityEngine;

public class GroundedMovState : MovementState
{
    public GroundedMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
    {
        
    }

    public override void EnterState()
    {
        //Debug.Log("enter grounded");
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
        }

        // TODO "soft landing" logic
    }

    public override void ExitState()
    {
        //Debug.Log("exit grounded");
    }

    public override MovementStateMachine.MovementStates GetNextState()
    {
        RaycastHit hit;

        if(MovementContext.noor.GetVelocity().magnitude >= MovementContext.noor.MaxWalkSpeed)
        {
            return MovementStateMachine.MovementStates.Running;
        }
        
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
        if(MovementContext.inputVector.sqrMagnitude <= 0.001)
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
        Vector3 currVel = MovementContext.noor.GetVelocity();
        //Debug.Log("input " + MovementContext.inputVector);

        Vector3 worldInput = (MovementContext.camera.transform.right * MovementContext.inputVector.x) +
                (forward * MovementContext.inputVector.y);
        
        worldInput = Quaternion.FromToRotation(Vector3.up, MovementContext.groundNormal) * worldInput;

        MovementContext.noor.SetVelocity(
                currVel + (worldInput * MovementContext.noor.GroundAcceleration * Time.deltaTime)
        );

        //Debug.Log("worldinput " + worldInput);
        //Debug.Log("velocity " + MovementContext.noor.GetVelocity());
        //Debug.Log("delta " + Time.deltaTime);

        if(MovementContext.noor.GetVelocity().magnitude <= 0.001) return;

        // translation
        MovementContext.noor.transform.Translate(MovementContext.noor.GetVelocity() * Time.deltaTime, Space.World);

        // rotation
        MovementContext.noor.transform.rotation = Quaternion.RotateTowards(
                MovementContext.noor.transform.rotation,
                Quaternion.LookRotation(Vector3.ProjectOnPlane(currVel,Vector3.up),Vector3.up),
                MovementContext.noor.RotationSpeed * Time.deltaTime
        );
    }
}
