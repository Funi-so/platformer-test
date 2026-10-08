using UnityEngine;

public class ApproachState: EnvironmentInteractionState
{
    float _elapsedTime = 5.0f;
    float _lerpDuration = 0.5f;
    float _approachDuration = 2.0f;
    float _approachWeight = 0.5f;
    float _approachRotationWeight = 0.75f;
    float _rotationSpeed = 500f;
    float _riseDistanceThreshold = .5f;
    public ApproachState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }
    public override void EnterState()
    {
        Debug.Log("Entrando Estado de Approach");
        _elapsedTime = 0.0f;
    }
    public override void ExitState(){}
    public override void UpdateState(){
        Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
        _elapsedTime += Time.deltaTime;

        Context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIKTargetTransform.rotation, expectedGroundRotation, _rotationSpeed * Time.deltaTime);

        Context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, 
        _approachRotationWeight, _elapsedTime / _lerpDuration);
        Context.CurrentIKConstraint.weight = Mathf.Lerp(Context.CurrentIKConstraint.weight, 
        _approachWeight, _elapsedTime / _lerpDuration);
    }
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = _elapsedTime >= _approachDuration;
        if (isOverStateLifeDuration || CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }

        bool isWithinArmsReach = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
        Context.CurrentShoulderTransform.position)< _riseDistanceThreshold;
        bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isClosestPointOnColliderReal && isWithinArmsReach)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Rise;
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
