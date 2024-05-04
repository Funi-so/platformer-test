using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    [Header("Base Movement Settings")]
    public float speed = 6f;
    public float turnSmoothTime = 0.05f;

    [Header("Jump & Fall Settings")]
    public float gravity = -9.18f;
    public float increasedFallForce = 1.2f;
    public float jumpHeight = 3f;
    public float jumpHoldForce = 1.5f;
    public float jumpHoldTime = 0.25f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.1f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.1f;

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

    void Update()
    {
        Fall();
        Move();
        Jump();
    }

    void Fall()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            jumpWindow = Time.time + coyoteTime;
            fallingFromJump = false;
            midJump = false;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        if (fallingFromJump)
        {
            controller.Move(velocity * increasedFallForce * Time.deltaTime);
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }
    void Move()
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

    void Jump()
    {

        if (Input.GetButtonDown("Jump"))
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
}
