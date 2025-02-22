using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerRb : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float walkSpeed;
    public float sprintSpeed;

    public float dashSpeed;
    public float wallrunSpeed;
    public float slideSpeed;
    public float climbSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float boostFactor;
    public float walkFactor;
    public float sprintStopFactor;
    public float dashSpeedChangeFactor;
    public float slopeIncreaseMultiplier;
    private float startBoostFactor;

    public float maxYSpeed;
    public float groundDrag;

    private bool pressingInputKeys;
    private float turnSmoothTime = 0.05f;
    private float turnSmoothVelocity;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode walkKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Jump")]
    public float jumpForce;
    public float jumpHoldForce;
    public float jumpCooldown;

    public float airMultiplier;
    public float gravityMultiplier;

    public float jumpStartTime;
    private float jumpTime;
    private float jumpWindow;
    public float coyoteTime;
    public float jumpBufferTime;
    private float jumpBufferTimer;
    private bool isJumping;

    private bool jumpMovement; // habilita executar movimento do pulo
    private bool holdJump; // habilita executar movimento do pulo segurado

    bool jumpBuffer;
    bool fallingFromJump;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    float startYScale;


    [Header("Ground Check")]
    public float playerHeight;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    bool exitingSlope = false;
    RaycastHit slopeHit;

    [Header("Ledge Hang Settings")]
    public LayerMask canGrabOnto;
    public float downRayLength = 1.5f;
    public float fwdRayLength = .7f;
    public float forwardOffset = -.1f;
    public float upOffset = -1f;

    public float ledgeJumpTime;
    private float ledgeJumpTimer;

    [Header("References")]
    public Transform orientation;
    public Animator animator;
    public Camera cam;
    public Climbing climbingScript;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startBoostFactor = boostFactor;
        startYScale = transform.localScale.y;
    }

    void Update()
    {
        //ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundDistance, groundMask);
        if (isGrounded)
        {
            jumpWindow = Time.time + coyoteTime;
        }
        if (state == MovementState.walking || state == MovementState.crouching || state == MovementState.sprinting)
        {
             rb.drag = groundDrag;
        }
        else
        {
             rb.drag = 0;
        }

        rb.rotation = orientation.rotation;

        StateHandler();
        SpeedControl();
        MyInput();

        Jump();
        LedgeGrab();

        Physics.SyncTransforms();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        //Debug.Log("On Slope = " + OnSlope() + ", Speed = " + rb.velocity.magnitude + ", Movespeed = " + moveSpeed);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(horizontalInput != 0f || verticalInput != 0f)
        {
            pressingInputKeys = true;
        }
        else 
        {
            pressingInputKeys = false;
        }

        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouching = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            crouching = false;
        }

        if(hanging && verticalInput>0) { LedgeJump(); }
    }

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        dashing,
        crouching,
        sliding,
        climbing,
        wallrunning,
        hanging,
        air
    }

    bool keepMomentum;

    public bool dashing;
    public bool crouching;
    public bool sliding;
    public bool wallrunning;
    public bool climbing;
    public bool hanging;

    public bool freeze;
    public bool unlimited;
    public bool restricted;

    void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
        }

        else if (unlimited)
        {
            state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        }

        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            boostFactor = dashSpeedChangeFactor;
            keepMomentum = true;
        }

        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }

        else if (hanging)
        {
            state = MovementState.hanging;
            ledgeJumpTimer -= Time.deltaTime;
        }

        else if (sliding)
        {
            state = MovementState.sliding;

            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                boostFactor = startBoostFactor;
                keepMomentum = true;
            }
            else
            { 
                desiredMoveSpeed = sprintSpeed;
            }
        }
        
        else if(wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        //Caminhada
        else if (Input.GetKey(walkKey) && isGrounded)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            if(pressingInputKeys)
            boostFactor = walkFactor;
            else
            boostFactor = sprintStopFactor;
            keepMomentum = true;
        }
        else if ( isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        //Corrida
        //Ar
        else {
            state = MovementState.air;
        }



        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else 
            {
                moveSpeed = desiredMoveSpeed;
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;

            if(Mathf.Abs(desiredMoveSpeed - moveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
        }

        //Checa se a desiredMoveSpeed mudou dr�sticamente
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {

        // lerp movespeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;
            
        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time/difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * boostFactor * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * boostFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if(jumpMovement) JumpMovement();
        if(holdJump) JumpHoldMovement();

        if(restricted) { return; }
        //if(climbingScript.exitingWall) { return; }

        //moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        Vector3 dir = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        // Em uma slope (chão declinado)
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 80f, ForceMode.Force); 

            if(rb.velocity.y < 0 && (horizontalInput> 0 || verticalInput > 0))
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (pressingInputKeys)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (isGrounded)
            {
                rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else if (!isGrounded)
            {
                rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        } 

        if (!wallrunning) { rb.useGravity = !OnSlope(); }
    }

    private void SpeedControl()
    {
        // Limita velocidade em uma slope

        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }


        // Limita velocidade no ch�o ou no ar

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // Limita velocidade em Y
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump()
    {

        //Pulo (Caso aperte o botao ou pelo Jump Buffer)
        if (Input.GetButtonDown("Jump"))
        {
            if(hanging) LedgeJump();

            if (Time.time <= jumpWindow || jumpBuffer && isGrounded && Time.time <= jumpBufferTimer) 
            { 
                exitingSlope = true;
                isJumping = true;
                jumpTime = jumpStartTime;

                //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                jumpMovement = true;

                //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                Invoke(nameof(ResetJump), jumpCooldown);

            }

            //Caso aperte bot�o de pulo fora do ch�o come�a a contar timer do jump buffer
            else if (!isGrounded && Time.time >= jumpWindow)
            {
                jumpBuffer = true;
                jumpBufferTimer = Time.time + jumpBufferTime;
            }

            else {
                jumpMovement = false;
            }
        }

        //Se tiver apertado bot�o de pulo fora do ch�o, quando encostar no ch�o vai resetar ele (e pular, c�digo ali em cima)
        if (jumpBuffer && isGrounded && Time.time <= jumpBufferTimer)
        {
            jumpBuffer = false;
        }

        //E caso apertar o bot�o de pulo cedo demais n�o funciona
        else if (jumpBuffer && Time.time >= jumpBufferTimer)
        {
            jumpBuffer = false;
        }

        //Se continuar apertando o bot�o de pulo, fica no ar por mais tempo
        if (Input.GetButton("Jump") && isJumping)
        {
            //rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Force);
            holdJump = true;
            jumpTime -= Time.deltaTime;
            
        }
        else
        {
            isJumping = false;
            holdJump = false;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
           
        //Se estiver caindo, a "gravidade" aumenta
        if(rb.velocity.y < 0 && !OnSlope() && !hanging && !climbing) 
            { 
                rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Force);
            }
    }
    private void JumpMovement(){
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        jumpMovement = false;
    }

    private void JumpHoldMovement(){rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Force);}
    private void ResetJump()
    {
        //readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction) 
    { 
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    void LedgeGrab()
    {
        if (rb.velocity.y < 0 && !hanging)
        {
            RaycastHit downHit;
            Vector3 lineDownStart = transform.position + Vector3.up * downRayLength + transform.forward;
            Vector3 lineDownEnd = transform.position + Vector3.up * fwdRayLength + transform.forward;
            Physics.Linecast(lineDownStart, lineDownEnd, out downHit, canGrabOnto);
            Debug.DrawLine(lineDownStart, lineDownEnd, Color.red);

            if (downHit.collider != null)
            {
                RaycastHit fwdHit;
                Vector3 lineFwdStart = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z);
                Vector3 lineFwdEnd = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z) + transform.forward;
                Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, canGrabOnto);

                if (fwdHit.collider != null)
                {
                    // Hanging on Ledge
                    rb.velocity = Vector3.zero;
                    rb.useGravity = false;

                    hanging = true;
                    restricted = true;

                    Vector3 hangPos = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
                    Vector3 offset = transform.forward * forwardOffset + transform.up * upOffset;
                    hangPos += offset;
                    transform.position = hangPos;
                    transform.forward = new Vector3(-fwdHit.normal.x, 0f, -fwdHit.normal.z);

                    ledgeJumpTimer = ledgeJumpTime;
                }
            }
        }
    }

    void LedgeJump()
    {
        if(ledgeJumpTimer <= 0)
        { 
            rb.useGravity = true;
            hanging = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            StartCoroutine(EnableCanMove(.25f));
        }
    }
    IEnumerator EnableCanMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        restricted = false;
    }
}
