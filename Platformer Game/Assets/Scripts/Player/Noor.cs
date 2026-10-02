using System;
using System.Collections.Generic;
using UnityEngine;

public class Noor : Singleton<Noor>
{
    private Vector2 _moveInput;
    private bool _jumpBuffer;
    private Vector3 _velocity;
    [SerializeField] private Animator _noorAnimator;
    [SerializeField] private InputHandler IH;
    [Space(5)]
    [Header("Movement Knobs")]
    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundStopping;
    [SerializeField] private float _groundBreaking;
    [SerializeField] private float _maxRunSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpLockTime;
    private float _jumpLockTimer;
    [SerializeField] private LayerMask _collisionLayers;

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
                _velocity.y = 0;
                _noorAnimator.SetBool("Grounded", true);
            }else{
                _velocity.y -= 10* Time.fixedDeltaTime;
                _noorAnimator.SetBool("Grounded", false);
            }
        }
        #region Comportamento no chão
        if (_noorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            //Debug.Log("Grounded");
            float vInputDot = Vector2.Dot(_moveInput, new Vector2(_velocity.x,_velocity.z));
            // movendo com a velocidade
            if(vInputDot >= -0.5f || _velocity.magnitude < 0.01)
            {
                //Debug.Log("Indo");
                // parada natural
                if(_moveInput.magnitude < _velocity.magnitude / _maxRunSpeed)
                {
                    //Debug.Log("Indo menos");
                    _velocity = _velocity.normalized * MathF.Max(0, _velocity.magnitude - _groundStopping);
                }
                else // movimento normal
                {
                    //Debug.Log("Indo mais");
                    _velocity += new Vector3(_moveInput.x, 0, _moveInput.y) * _groundAcceleration;
                }

                // rotacao mantendo vel
                Vector3.RotateTowards(in _velocity, new Vector3(_moveInput.x, 0, _moveInput.y), 0.5f, 0);
            }
            else // movendo contra a velocidade
            {
                //Debug.Log("Turnaround");
                _velocity = _velocity.normalized * MathF.Max(0, _velocity.magnitude - _groundBreaking);
            }

            // clamp pra max speed
            _velocity = _velocity.magnitude>_maxRunSpeed? _velocity.normalized*_maxRunSpeed : _velocity;

            // olha pra onde ta andando sua sonsa
            if(_velocity.sqrMagnitude > 0.01f)
                _noorAnimator.transform.Rotate(Vector3.up, Vector3.SignedAngle(_noorAnimator.transform.forward, _velocity, Vector3.up));

            //pulin :>
            if(_jumpBuffer == true)
            {
                _jumpBuffer = false;
                _velocity += Vector3.up * _jumpForce;
                _jumpLockTimer = _jumpLockTime;
                _noorAnimator.SetTrigger("Jump");
            }
            #endregion
        }

        // aplicacao de movimento
        //Debug.Log("translate (pfv)");
        transform.Translate(_velocity * Time.fixedDeltaTime);
    }

    private bool GroundCheck()
    {
        //Debug.Log("GroundCheck");
        
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position + Vector3.up*0.1f, Vector3.down);
        Debug.DrawLine(ray.origin, ray.origin + Vector3.down*0.2f);
        if(Physics.Raycast(ray, out hitInfo, 0.2f, _collisionLayers.value))
        {
            Debug.Log("Yup, Ground");
            if(hitInfo.point.y > transform.position.y)
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            return true;
        }
        else
            //Debug.Log("Nope Ground");
            return false;
    }
}
