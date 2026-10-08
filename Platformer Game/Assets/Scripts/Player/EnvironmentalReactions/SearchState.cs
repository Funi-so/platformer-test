using UnityEngine;

public class SearchState: EnvironmentInteractionState
{
    public float _approachDistanceThreshold = 2.0f;
    public SearchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("Entrando no estado Search");
    }
    public override void ExitState(){}
    public override void UpdateState()
    {
    }
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        if(CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        bool isCloseToTarget = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,Context.RootTransform.position)<_approachDistanceThreshold;
        bool isClosestPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if(isClosestPointOnColliderValid && isCloseToTarget)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Approach;
        }
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other)
    {
        StartIKTargetPositionTracking(other);
    }
    public override void OnTriggerStay(Collider other)
    {
        UpdateIKTargetPosition(other);
    }
    public override void OnTriggerExit(Collider other)
    {
        ResetIKTargetPositionTracking(other);
    }
}