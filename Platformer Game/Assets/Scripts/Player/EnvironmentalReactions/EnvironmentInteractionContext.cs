using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    public enum EBodySide
    {
        RIGHT,
        LEFT
    }
    private Noor _noor;
    private TwoBoneIKConstraint _leftIKConstraint;
    private TwoBoneIKConstraint _rightIKConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    private Transform _rootTransform;
    private Vector3 _leftOriginalTargetPosition;
    private Vector3 _rightOriginalTargetPosition;

    public EnvironmentInteractionContext(Noor noor,TwoBoneIKConstraint leftIkConstraint,TwoBoneIKConstraint rightIkConstraint,MultiRotationConstraint 
    leftMultiRotationConstraint,MultiRotationConstraint rightMultiRotationConstraint, Transform rootTransform)
    {
        _noor = noor;
        _leftIKConstraint = leftIkConstraint;
        _rightIKConstraint = rightIkConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rootTransform = rootTransform;
        _leftOriginalTargetPosition = _leftIKConstraint.data.target.transform.localPosition;
        _rightOriginalTargetPosition = _rightIKConstraint.data.target.transform.localPosition;
        OriginalTargetRotation = _leftIKConstraint.data.target.rotation;

        CharacterShoulderHeight = leftIkConstraint.data.root.transform.position.y;
        SetCurrentSide(Vector3.positiveInfinity);
    }
    public Noor noor => _noor;
    public TwoBoneIKConstraint LeftIKConstraint => _leftIKConstraint;
    public TwoBoneIKConstraint RightIKConstraint =>_rightIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint =>_leftMultiRotationConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint =>_rightMultiRotationConstraint;
    public Transform RootTransform => _rootTransform;

    public float CharacterShoulderHeight {get;private set;}

    public Collider CurrentIntersectingCollider{get;set;}
    public TwoBoneIKConstraint CurrentIKConstraint {get; private set;}
    public MultiRotationConstraint CurrentMultiRotationConstraint{get; private set;}
    public Transform CurrentIKTargetTransform{get; private set;}
    public Transform CurrentShoulderTransform{get; private set;}
    public EBodySide CurrentBodySide{get; private set;}
    public Vector3 ClosestPointOnColliderFromShoulder{get;set;} = Vector3.positiveInfinity;
    public float InteractionPointYOffset{get;set;}=0f;
    public float ColliderCenterY{get;set;}
    public Vector3 CurrentOriginalTargetPosition{get;private set;}
    public Quaternion OriginalTargetRotation{get;private set;}
    public float LowestDistance{get;set;}=Mathf.Infinity;
    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIKConstraint.data.root.transform.position;
        
        Vector3 rightShoulder = _rightIKConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder)< Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            Debug.Log("Left Side is Closer");
            CurrentBodySide = EBodySide.LEFT;
            CurrentIKConstraint = _leftIKConstraint;
            CurrentMultiRotationConstraint= _leftMultiRotationConstraint;
            CurrentOriginalTargetPosition = _leftOriginalTargetPosition;
        } else
        {
            Debug.Log("Right Side is Closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIKConstraint = _rightIKConstraint;
            CurrentMultiRotationConstraint= _rightMultiRotationConstraint;
            CurrentOriginalTargetPosition = _rightOriginalTargetPosition;
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
    }
}
