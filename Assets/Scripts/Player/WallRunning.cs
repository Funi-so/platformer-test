using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public KeyCode jumpKey;
    public LayerMask isWall;
    public LayerMask isGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public FirstPersonCamera cam;
    private PlayerRb pm;
    private Rigidbody rb;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    void Start()
    {
        pm = GetComponent<PlayerRb>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //if (GameManager.instance.hasWallRun)
        {
            CheckforWall();
            StateMachine();
        }
    }

    void FixedUpdate()
    {
        if(pm.wallrunning)
            WallRunMovement();
    }

    void CheckforWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, isWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, isWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, isGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            //start wall run

            if (!pm.wallrunning) 
            { 
                StartWallRun();
            }

            // wall jump
            if(pm.wallrunning && Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }

            //fall timer
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if(wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

        }

        // exiting wall
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if(wallLeft || wallRight)
            {
                exitWallTimer = exitWallTime;
            }

            if(exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // none

        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    void StartWallRun()
    {
        pm.wallrunning = true;
        //pm.canMove = false;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Camera effects
        cam.DoFov(95f);
        if(wallLeft) { cam.DoTilt(-4f); }
        if (wallRight) { cam.DoTilt(4f); }
    }

    void WallRunMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //push to wall force
        if(!(wallLeft && horizontalInput > 0) && !(wallLeft && horizontalInput < 0)) 
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        //weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true;

        //camera effects reset
        cam.DoFov(90f);
        cam.DoTilt(0f); 
    }

    private void WallJump()
    {
        if (pm.isGrounded) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        //pm.velocity += forceToApply;
    }
}
