using UnityEngine;

public class GroundedMovState : MovementState
{

    public GroundedMovState(MovementStateMachine.MovementStates stateKey) : base(stateKey)
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
        RaycastHit hit;
        Ray ray = new Ray(MovementContext.noor.transform.position + Vector3.up,Vector3.down);
        if(Physics.Raycast(ray, out hit, 1.2f, MovementContext.noor.GetCollisionLayers()))
        {
            MovementContext.groundNormal = hit.normal;
            MovementContext.noor.transform.position = new Vector3(MovementContext.noor.transform.position.x,
                    Mathf.Max(hit.point.y,MovementContext.noor.transform.position.y),
                    MovementContext.noor.transform.position.z);
        }
        return StateKey;
    }

    public override void UpdateState()
    {
        Vector3 forward = MovementContext.camera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        Vector3 worldInput = (MovementContext.camera.transform.right * MovementContext.inputVector.x) +
                (forward * MovementContext.inputVector.y);
        
        worldInput = Quaternion.FromToRotation(Vector3.up, MovementContext.groundNormal) * worldInput;

        MovementContext.noor.transform.Translate(worldInput * Time.deltaTime);


    }
}
