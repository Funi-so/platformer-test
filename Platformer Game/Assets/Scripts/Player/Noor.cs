using System.Collections.Generic;
using UnityEngine;

public class Noor : Singleton<Noor>
{
    private Vector2 _moveInput;
    private Vector3 _velocity;
    [SerializeField] private Animator NoorAnimator;
    [SerializeField] private InputHandler InputHandlerSO;
    [Space(5)]
    [Header("Movement Knobs")]
    [SerializeField]
    private float _groundAcceleration;

    public void SetMoveInput(Vector2 input)
    {
        //Debug.Log("moveInput received");
        _moveInput = input;
    }

    void FixedUpdate()
    {
        // aceleração de chão
        if (NoorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            //Debug.Log("change vel");
            if(_moveInput == Vector2.zero)
            {
                _velocity -= _velocity/2;
            }else{
                _velocity += new Vector3(_moveInput.x, 0, _moveInput.y) * _groundAcceleration;
            }
        }

        // aplicacao de movimento
        //Debug.Log("translate (pfv)");
        transform.Translate(_velocity * Time.fixedDeltaTime);
    }

    private bool GroundCheck()
    {
        return true;
    }
}
