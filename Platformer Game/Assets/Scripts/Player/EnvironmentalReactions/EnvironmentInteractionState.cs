using NUnit.Framework;
using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    protected EnvironmentInteractionContext Context;
    private float _movingAwayOffset =0.005f;
    private bool _shouldReset;
    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState stateKey) : base(stateKey)
    {
        Context = context;
    }

    protected bool CheckShouldReset()
    {
        if (_shouldReset)
        {
            Context.LowestDistance = Mathf.Infinity;
            _shouldReset = false;
            return true;
        }

        bool isPlayerStopped = Context.noor._velocity == Vector3.zero;
        bool isMovingAway = CheckIsMovingAway();
        bool isBadAngle = CheckIsBadAngle();
        bool isPlayerJumping = Mathf.Round(Context.noor._velocity.y) >1;

        if (isPlayerStopped || isMovingAway || isBadAngle || isPlayerJumping)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;
    }
    protected bool CheckIsMovingAway()
    {
        float currentDistanceToTarget = Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromShoulder);

        bool isSearchingForNewInteraction = Context.CurrentIntersectingCollider==null;
        if (isSearchingForNewInteraction)
        {
            return false;
        }

        bool isGettingCloserToTarget= currentDistanceToTarget <= Context.LowestDistance;
        if (isGettingCloserToTarget)
        {
            Context.LowestDistance = currentDistanceToTarget;
            return false;
        }
        
        bool isMovingAwayFromTarget= currentDistanceToTarget > Context.LowestDistance + _movingAwayOffset;
        if (isMovingAwayFromTarget)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        return false;
    }
    protected  bool CheckIsBadAngle()
    {
        if (Context.CurrentIntersectingCollider == null)
        {
            return false;
        }
        Vector3 targetDirection = Context.ClosestPointOnColliderFromShoulder - Context.CurrentShoulderTransform.position;
        Vector3 shoulderDirection = Context.CurrentBodySide == EnvironmentInteractionContext.EBodySide.RIGHT? 
        Context.RootTransform.right : -Context.RootTransform.right;

        float dotProduct = Vector3.Dot(shoulderDirection, targetDirection.normalized);
        bool isBadAngle = dotProduct <0;

        return isBadAngle;
    }
    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIKTargetPositionTracking(Collider intersectingCollider)
    {
        if (Context.CurrentIntersectingCollider==null)
        {   
            Context.CurrentIntersectingCollider = intersectingCollider;
            Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
            Context.SetCurrentSide(closestPointFromRoot);
        
            SetIKTargetPosition();
        }
    }
    protected void UpdateIKTargetPosition(Collider intersectingCollider)
    {
        if(intersectingCollider==Context.CurrentIntersectingCollider){SetIKTargetPosition();}
    }
    protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromShoulder=Vector3.positiveInfinity;
            _shouldReset = true;
        }
    }

    private void SetIKTargetPosition()
    {
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider, 
        new Vector3(Context.CurrentShoulderTransform.position.x, Context.CharacterShoulderHeight, Context.CurrentShoulderTransform.position.z));

        Vector3 rayDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
        Vector3 normalizedRayDirection = rayDirection.normalized;
        float offsetDistance = .05f;
        Vector3 offset = normalizedRayDirection*offsetDistance;

        Vector3 offsetPosition = Context.ClosestPointOnColliderFromShoulder+offset;
        Context.CurrentIKTargetTransform.position = new Vector3(offsetPosition.x, Context.InteractionPointYOffset, offsetPosition.z);
    }
}
