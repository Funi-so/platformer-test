using System;
using System.Collections.Generic;
using UnityEngine;
using LifelikeMotion.IKFootPlacement;

public class Noor : Singleton<Noor>
{
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private Vector2 _moveInput;
    private bool _jumpBuffer;
    [HideInInspector] public Vector3 _velocity;
    private Vector3 _groundNormal;
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
    [SerializeField] private float _jumpLockTime;
    private float _jumpLockTimer;
    [Space(5)]
    [Header("Collision Knobs")]
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private float _innerColRadius;
    [SerializeField] private float _innerColHeight;
    [SerializeField] private float _outerColRadius;
    [SerializeField] private float _outerColHeight;

    public void SetMoveInput(Vector2 input)
    {
        //Debug.Log("moveInput received");
        _moveInput = input;
    }

    public void SetJumpBuffer()
    {
        //Debug.Log("jump input received");
        _jumpBuffer = true;
    }

    void FixedUpdate()
    {
        if (_jumpLockTimer > 0)
        {
            _jumpLockTimer -= Time.fixedDeltaTime;
        }else{
            if(GroundCheck()){
                _velocity += _groundNormal * -Vector3.Dot(_velocity.normalized, _groundNormal) * _velocity.magnitude;
                _noorAnimator.SetBool(GroundedHash, true);
                iKFootPlacement.isGrounded = true;
            }else{
                _velocity.y -= _gravityForce* Time.fixedDeltaTime;
                _noorAnimator.SetBool(GroundedHash, false);
                iKFootPlacement.isGrounded = false;
            }
        }

        if (_noorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            PhysCalc_Grounded();
        }
        
        // fala pro ik se esta mechendo ou nao (creio que independe do grounded)
        if (_velocity.sqrMagnitude > 0.01f) iKFootPlacement.isMoving = true;
                else iKFootPlacement.isMoving = false;

        // aplicacao de movimento
        //Debug.Log("translate (pfv)");
        transform.Translate(_velocity * Time.fixedDeltaTime);
    }

    private bool GroundCheck()
    {
        //Debug.Log("GroundCheck");
        
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position + Vector3.up*0.85f, Vector3.down);
        Debug.DrawLine(ray.origin, ray.origin + Vector3.down*0.85f);
        if(Physics.Raycast(ray, out hitInfo, 0.9f, _collisionLayers.value))
        {
            //Debug.Log("Yup, Ground");
            if(hitInfo.point.y > transform.position.y)
                transform.position = new Vector3(transform.position.x, hitInfo.point.y+0.05f, transform.position.z);
            _groundNormal = hitInfo.normal;
            return true;
        }
        else
            //Debug.Log("Nope Ground");
            return false;
    }

    private void PhysCalc_Grounded()
    {
        //Debug.Log("Grounded");
        float vInputDot = Vector2.Dot(_moveInput, new Vector2(_velocity.x, _velocity.z));
        Vector3 inputInWorld = Quaternion.FromToRotation(Vector3.up, _groundNormal) * new Vector3(_moveInput.x, 0, _moveInput.y);

        // movendo com a velocidade
        if (vInputDot >= 0f || _velocity.magnitude < 0.01)
        {
            //Debug.Log("Indo");
            // parada natural
            if (_moveInput.magnitude < _velocity.magnitude / _maxRunSpeed)
            {
                //Debug.Log("Indo menos");
                _velocity = _velocity.normalized * MathF.Max(0, _velocity.magnitude - _groundStopping);
            }
            else // movimento normal
            {
                //Debug.Log("Indo mais");
                Vector3 movAddVec = inputInWorld  * _groundAcceleration;
                _velocity += movAddVec;
            }

            // rotacao mantendo vel
            _velocity = Vector3.RotateTowards(_velocity, Quaternion.FromToRotation(Vector3.up, _groundNormal) * inputInWorld, 2f*Time.fixedDeltaTime, 0.1f*Time.fixedDeltaTime);
        }

        // movendo contra a velocidade
        else if (vInputDot >= -0.75f)
        {
            // rotacao mantendo vel
            _velocity = Vector3.RotateTowards(_velocity, inputInWorld, 2f*Time.fixedDeltaTime, 0.1f*Time.fixedDeltaTime);
        }

        // turnaround
        else 
        {
            //Debug.Log("Turnaround");
            _velocity = _velocity.normalized * MathF.Max(0, _velocity.magnitude - _groundBreaking);
        }

        // clamp pra max speed
        _velocity = _velocity.magnitude > _maxRunSpeed ? _velocity.normalized * _maxRunSpeed : _velocity;

        // olha pra onde ta andando sua sonsa
        transform.GetChild(0).Rotate(Vector3.up, Vector3.SignedAngle(transform.GetChild(0).forward, new Vector3(_velocity.x,0, _velocity.z), Vector3.up));  // gambiarra
        _noorAnimator.SetFloat("Speed", new Vector2(_velocity.x, _velocity.z).magnitude / _maxRunSpeed);
        _noorAnimator.SetFloat("Y Speed", _velocity.y);

        // pulin :>
        if (_jumpBuffer == true)
        {
            _jumpBuffer = false;
            _velocity += Vector3.up * _jumpForce;
            _jumpLockTimer = _jumpLockTime;
            _noorAnimator.SetTrigger("Jump");
            iKFootPlacement.jumped = true;
        }
    }

    private void WallCheck()
    {
        //RaycastHit hitInfo;
    }
}
