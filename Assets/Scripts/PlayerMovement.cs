using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    [Header("Base Movement Settings")]
    public float speed = 6f;
    float turnSmoothTime = 0.05f;

    [Header("Jump & Fall Settings")]
    public float gravity = -9.18f;
    public float increasedFallForce = 2f;
    public float jumpHeight = 3f;
    public float jumpHoldForce = 1.5f;
    public float jumpHoldTime = 0.25f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.1f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.1f;

    [Header("Ledge Hang Settings")]
    public float downRayLength = 1.5f;
    public float fwdRayLength = .7f;
    public float forwardOffset = -.1f;
    public float upOffset= -1f;


    private Vector3 dir;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private float maxJumpBufferTime = 0f;
    private float jumpWindow;
    private float jumpTime;
    private bool isJumping;
    private bool midJump;
    private bool fallingFromJump;
    private bool isGrounded;
    private bool jumpBuffer = false;
    bool hanging;
    bool useGravity = true;
    bool canMove = true;

    void Update()
    {
        Fall();
        Move();
        Jump();
        LedgeGrab();
    }

    void Fall()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            jumpWindow = Time.time + coyoteTime;
            fallingFromJump = false;
            midJump = false;

            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        if (useGravity) { velocity.y += gravity * Time.deltaTime; }

        if (fallingFromJump){ controller.Move(velocity * increasedFallForce * Time.deltaTime); }
        else { controller.Move(velocity * Time.deltaTime); }
    }
    void Move()
    {
        if (canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            dir = new Vector3(horizontal, 0f, vertical).normalized;

            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }
    }

    void Jump()
    {

        if (Input.GetButtonDown("Jump"))
        {
            if (hanging)
            {
                useGravity = true;
                hanging = false; 

                isJumping = true;
                midJump = true;
                jumpTime = jumpHoldTime;
                velocity.y = Mathf.Sqrt(jumpHoldForce * -2f * gravity);

                StartCoroutine(EnableCanMove(.25f));

            } else
            {

                if (Time.time <= jumpWindow && !isJumping)
                {
                    isJumping = true;
                    midJump = true;
                    jumpTime = jumpHoldTime;
                    velocity.y = Mathf.Sqrt(jumpHoldForce * -2f * gravity);
                    jumpWindow = 0f;
                }
                else if (!isGrounded && Time.time >= jumpWindow)
                {
                    jumpBuffer = true;
                    maxJumpBufferTime = Time.time + jumpBufferTime;
                }
            }
        }

        if (jumpBuffer && isGrounded && Time.time <= maxJumpBufferTime)
        {
            isJumping = true;
            midJump = true;
            jumpTime = jumpHoldTime;
            velocity.y = Mathf.Sqrt(jumpHoldForce * -2f * gravity);
            jumpBuffer = false;
        }
        else if (jumpBuffer && Time.time >= maxJumpBufferTime)
        {
            jumpBuffer = false;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTime > 0)
            {
                velocity.y = Mathf.Sqrt(jumpHoldForce * -2f * gravity);
                jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (isJumping) { midJump = true; }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (midJump && velocity.y < 0)
        {
            fallingFromJump = true;
        }
        

        if (jumpBuffer && Time.time >= maxJumpBufferTime)
        {
            jumpBuffer = false;
        }
    }

    IEnumerator EnableCanMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canMove = true;
    }
    void LedgeGrab()
    {
        if(velocity.y < 0 && !hanging)
        {
            RaycastHit downHit;
            Vector3 lineDownStart = (transform.position + Vector3.up * downRayLength) + transform.forward;
            Vector3 lineDownEnd = (transform.position + Vector3.up * fwdRayLength) + transform.forward;
            Physics.Linecast(lineDownStart, lineDownEnd, out downHit, groundMask);
            Debug.DrawLine(lineDownStart, lineDownEnd);

            if(downHit.collider != null)
            {
                RaycastHit fwdHit;
                Vector3 lineFwdStart = new Vector3(transform.position.x, downHit.point.y-0.1f, transform.position.z);
                Vector3 lineFwdEnd = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z)+ transform.forward;
                Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, groundMask);
                Debug.DrawLine(lineFwdStart, lineFwdEnd);

                if (fwdHit.collider != null)
                {
                    velocity = Vector3.zero;

                    hanging = true;
                    useGravity = false;
                    canMove = false;

                    Vector3 hangPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
                    Vector3 offset = transform.forward * forwardOffset + transform.up * upOffset;
                    hangPos += offset;
                    transform.position = hangPos;
                    transform.forward = -fwdHit.normal;
                }
            }
        }
    }
}
