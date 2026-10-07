using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private Noor _noor;
    private TwoBoneIKConstraint _leftIkConstraint;
    private TwoBoneIKConstraint _rightIkConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;

    public EnvironmentInteractionContext(Noor noor,TwoBoneIKConstraint leftIkConstraint,TwoBoneIKConstraint rightIkConstraint,MultiRotationConstraint 
    leftMultiRotationConstraint,MultiRotationConstraint rightMultiRotationConstraint)
    {
        _noor = noor;
        _leftIkConstraint = leftIkConstraint;
        _rightIkConstraint = rightIkConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
    }
    public Noor noor => _noor;
    public TwoBoneIKConstraint LeftIkConstraint => _leftIkConstraint;
    public TwoBoneIKConstraint RightIkConstraint =>_rightIkConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint =>_leftMultiRotationConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint =>_rightMultiRotationConstraint;
}
