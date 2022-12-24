using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody rb;
    [Header("Move Player")]
    private CharacterController playerController;
    public float groundDrag;
    float horizontalInput;
    float verticalInput;
    public float moveSpeed;

    [Header("Crouch Player")]
    private float YScale;
    public float crouchScale = 2;
    public bool crouch = false;

    [Header("Inputs")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode wKey = KeyCode.W;

    public Transform orientation;

    Vector3 moveDirection;

    //Movement States
    public movementState state;
    public enum movementState
    {
        walking,
        sprinting,
        crouching
    }

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<CharacterController>();
        rb.freezeRotation = true; 
        YScale = transform.localScale.y;

    }

    void Update()
    {
        rb.drag = groundDrag;

        speedControl();
        Inputs();
        setState();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    //Take in user inputs

    void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(crouchKey))
        {
            crouch = true;
            transform.localScale = new Vector3(transform.localScale.x,crouchScale,transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if(Input.GetKeyUp(crouchKey))
        {
            crouch = false;
            transform.localScale = new Vector3(transform.localScale.x,YScale,transform.localScale.z);
        }
    }

    //Move the actual player

    void MovePlayer()
    {
        //Store user input as a movement vector
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    //What is your current movement

    private void setState()
    {
        if(Input.GetKey(crouchKey))
        {
            state = movementState.crouching;
            moveSpeed = 3;
        }

        else if(Input.GetKey(sprintKey))
        {
            state = movementState.sprinting;
            moveSpeed = 10;
        }

        else
        {
            state =movementState.walking;
            moveSpeed = 5;
        }
    }

    //Control the speed of the player

    private void speedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude>moveSpeed)
        {
            Vector3 limitedVel =  flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}