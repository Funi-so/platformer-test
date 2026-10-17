using System;
using System.Collections.Generic;
using UnityEngine;
using LifelikeMotion.IKFootPlacement;

public class Noor : Singleton<Noor>
{
    private MovementStateMachine movementStateMachine;
    private Vector3 _velocity;
    public Vector3 GetVelocity(){return _velocity;}
    [SerializeField] private Animator _noorAnimator;
    [SerializeField] private IKFootPlacement iKFootPlacement;
    [SerializeField] private InputHandler IH;
    [Space(5)]
    [Header("Movement Knobs")]
    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundStopping;
    [SerializeField] private float _groundBreaking;
    [SerializeField] private float _maxRunSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityForce;
    [Space(5)]
    [Header("Collision Knobs")]
    [SerializeField] private LayerMask _collisionLayers;
    public LayerMask GetCollisionLayers() {return _collisionLayers;}
    
    
}
