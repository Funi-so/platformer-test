using System;
using System.Collections.Generic;
using UnityEngine;
using LifelikeMotion.IKFootPlacement;

public class Noor : Singleton<Noor>
{
    private MovementStateMachine movementStateMachine;
    private Vector3 _velocity;
    public Vector3 GetVelocity(){return _velocity;}
    public void SetVelocity(Vector3 value){_velocity = value;}
    [SerializeField] private Animator _noorAnimator;
    [SerializeField] private IKFootPlacement iKFootPlacement;
    [SerializeField] private InputHandler IH;
    [Header("Movement Knobs")]
    [field: SerializeField]
    public float GroundAcceleration {get; private set;}
    [field: SerializeField]
    public float GroundStopping {get; private set;}
    [field: SerializeField]
    public float GroundBreaking {get; private set;}
    [field: SerializeField]
    public float MaxWalkSpeed {get; private set;}
    [field: SerializeField]
    public float MaxRunSpeed {get; private set;}
    [field: SerializeField]
    public float RotationSpeed {get; private set;}
    [field: SerializeField]
    public float JumpForce {get; private set;}
    [field: SerializeField]
    public float GravityForce {get; private set;}

    [Space(5)]
    [Header("Collision Knobs")]
    [SerializeField] private LayerMask _collisionLayers;
    public LayerMask GetCollisionLayers() {return _collisionLayers;}

    void OnDestroy()
    {
        Debug.Log(gameObject + " | " + GetInstance().gameObject);
    }
}
