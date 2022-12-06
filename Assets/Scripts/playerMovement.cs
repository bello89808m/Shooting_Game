
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchScale;
    private float YScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode rightPeakKey = KeyCode.E;
    public KeyCode leftPeakKey = KeyCode.Q;

    [Header("Ground")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope")]
    public float maxSlope;
    public RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public movementState state;
    // Movement states
    public enum movementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        readyToJump = true;
        YScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
         if(grounded)
            rb.drag = groundDrag;
         else
            rb.drag = 0;

        myInput();
        speedControl();
        setState();
    }

    // Update is called multiple times per frame
    private void FixedUpdate()
    {
        movePlayer();
    }

    //******************************* In the update function ******************************
    
    // Inputs of the player
    private void myInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            jump();
            Invoke(nameof(resetJump),jumpCooldown);
        }

        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,crouchScale,transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
         if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,YScale,transform.localScale.z);
        }
    }

    // Control the speed of which the player is moving
    private void speedControl()
    {
        if(onSlope())
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if(flatVel.magnitude>moveSpeed)
            {
                Vector3 limitedVel =  flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    // Setting what state you are in
    private void setState()
    {
        if(Input.GetKey(crouchKey))
        {
            state = movementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = movementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state =movementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state =movementState.air;
        }
    }

    //******************************* In the fixedupdate function ******************************

    // What gets the player to move
    private void movePlayer()
    {
        //calculate player movement
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //on slope
        if(onSlope() && !exitingSlope)
        {
            rb.AddForce(slopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if(rb.velocity.y > 0)
                rb.AddForce(Vector3.down*80f,ForceMode.Force);
        }   
        //on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //on air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        rb.useGravity = !onSlope();
    }     

    //******************************* Called in different functions ******************************

    // Allow the player to jump
    private void jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Make sure that you can jump again
    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool onSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f+0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlope && angle != 0;
        }
        return false;
    }
    private Vector3 slopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection,slopeHit.normal).normalized;
    }
}
