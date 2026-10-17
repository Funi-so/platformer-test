using System.Data;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

public class EnvironmentInteractionStateMachine : BaseStateMachine<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    public float wingspan = 1.8f;
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    private EnvironmentInteractionContext _context;
    [SerializeField] private Noor _noor;
    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(_context !=null && _context.ClosestPointOnColliderFromShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestPointOnColliderFromShoulder, .03f);
        }
    }
    void Awake()
    {
        ValidateConstraints();
        _context = new EnvironmentInteractionContext(_noor, _leftIkConstraint, 
        _rightIkConstraint, _leftMultiRotationConstraint, _rightMultiRotationConstraint, transform.root);
        
        //ConstructEnvironmentDetectionCollider();
        _context.ColliderCenterY = GetComponent<BoxCollider>().center.y;
        InitializeStates();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIkConstraint, "Tá faltando o Left IK Constraint hein");
        Assert.IsNotNull(_rightIkConstraint, "Tá faltando o Right IK Constraint hein");
        Assert.IsNotNull(_leftMultiRotationConstraint, "Tá faltando o Left Multi Rotation Constraint hein");
        Assert.IsNotNull(_rightMultiRotationConstraint, "Tá faltando o Right Multi Rotation Constraint hein");
        Assert.IsNotNull(_noor, "Tá faltando o Controlador de Personagem hein");
    }

    private void InitializeStates()
    {
        States.Add(EEnvironmentInteractionState.Reset, new ResetEnvState(_context, EEnvironmentInteractionState.Reset));
        States.Add(EEnvironmentInteractionState.Search, new SearchEnvState(_context, EEnvironmentInteractionState.Search));
        States.Add(EEnvironmentInteractionState.Approach, new ApproachEnvState(_context, EEnvironmentInteractionState.Approach));
        States.Add(EEnvironmentInteractionState.Rise, new RiseEnvState(_context, EEnvironmentInteractionState.Rise));
        States.Add(EEnvironmentInteractionState.Touch, new TouchEnvState(_context, EEnvironmentInteractionState.Touch));
        CurrentState = States[EEnvironmentInteractionState.Reset];
    }

    private void ConstructEnvironmentDetectionCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(wingspan, wingspan, wingspan);
        boxCollider.center = new Vector3(_noor.transform.position.x, _noor.transform.position.y + (.9f+ .25f * wingspan), _noor.transform.position.y + (.5f * wingspan + .2f));
        boxCollider.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        EnvironmentInteractionState State = CurrentState as EnvironmentInteractionState; 
        State.OnTriggerEnter(other);
    }
    void OnTriggerStay(Collider other)
    {
        EnvironmentInteractionState State = CurrentState as EnvironmentInteractionState; 
        State.OnTriggerStay(other);
    }
    void OnTriggerExit(Collider other)
    {
        EnvironmentInteractionState State = CurrentState as EnvironmentInteractionState; 
        State.OnTriggerExit(other);
    }
}
