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
    private PlayerMovement pm;
    private PlayerStats stats;
    private CharacterController cc;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        cc = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (stats.hasWallRun)
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
        
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning) 
                StartWallRun();

        }
            if(wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if(wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if(pm.wallrunning && Input.GetKeyDown(jumpKey)) 
                WallJump();
       
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();
            if(exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
    }

    void StartWallRun()
    {
        pm.wallrunning = true;
        pm.canMove = false;

        wallRunTimer = maxWallRunTime;
    }

    void WallRunMovement()
    {
        pm.useGravity = false;
        pm.velocity.y = 0f;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;


        cc.Move(wallForward * wallRunForce);

        if(!(wallLeft && horizontalInput > 0) && !(wallLeft && horizontalInput < 0))
            cc.Move(-wallNormal * 2);
    }

    void StopWallRun()
    {
        pm.wallrunning = false;
        pm.useGravity = true;
        pm.canMove = true;
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //cc.Move(forceToApply);
        pm.velocity += forceToApply;
    }
}
