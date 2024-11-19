using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public bool useGravity;
    private float climbTimer;

    [Header("Climb Jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;

    [Header("References")]
    public Transform orientation;
    public PlayerRb pm;
    public Rigidbody rb;
    public LayerMask isWall;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if(pm.climbing && !exitingWall) { ClimbingMovement(); }
    }

    private void StateMachine()
    {
        //Climbing
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!pm.climbing && climbTimer > 0) { StartClimbing(); }

            if (climbTimer > 0) { climbTimer -= Time.deltaTime; }
            if (climbTimer < 0) { StopClimbing(); }
        }

        //Exiting
        else if (exitingWall)
        {
            if (pm.climbing) StopClimbing();

            if (exitWallTimer > 0) { exitWallTimer -= Time.deltaTime; }
            if (exitWallTimer < 0) 
            { 
                exitingWall = false; 
                pm.restricted = false;
            }
        }

        //No State
        else
        {
            if (pm.climbing) StopClimbing() ;
        }

        if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0) ClimbJump();

    }
    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, isWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if((wallFront && newWall) || pm.isGrounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing() 
    {
        rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
        rb.useGravity = useGravity;
        pm.climbing = true;
        pm.restricted = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        pm.climbing = false;
        rb.useGravity = true;
        pm.restricted = false;
    }

    private void ClimbJump()
    {
        if (pm.isGrounded) return;

        exitingWall = true;
        pm.restricted = true;

        exitWallTimer = exitWallTime;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
