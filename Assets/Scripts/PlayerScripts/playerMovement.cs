using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    private float x, z;
    public bool isSprinting;

    [Header("Gravity")]
    private Vector3 velocity;
    private float gravity = -5f;

    [Header("Crouch")]
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float standingSpeed;
    private float currentHeight;
    private float elapsedTime;
    public bool isCrouching;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    public float groundDistance;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;


    void Update()
    {
        playerMove();
    }

    public movementState state;
    public enum movementState
    {
        walking,
        crouching,
        sprinting,
        still
    }

    void playerMove()
    {
        //Ground check

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y= -2f;
        }

        //Gravity

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity *Time.deltaTime);

        //Moving the player

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        isSprinting = Input.GetKey(sprintKey);
        isCrouching = Input.GetKey(crouchKey);
        
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(Vector3.ClampMagnitude(move, 1.0f) * speed * Time.deltaTime);

        //Crouching

        if (isCrouching)
        {
            crouching();
        }else{
            standing();
        }

        //Movement State

        if (isCrouching && (x != 0 || z != 0))
        {
            state = movementState.crouching;
            speed = 2f;
        }else if(isSprinting && (x != 0 || z != 0)){
            state = movementState.sprinting;
            speed = 4f;

        }else if(x != 0 || z != 0){
            state = movementState.walking;
            speed = 3f;

        }else{
            state = movementState.still;
            speed = 0f;
        }
    }

    void crouching()
    {
        controller.height = Mathf.Lerp(currentHeight, 0.5f, Time.deltaTime * crouchingSpeed);
        if (controller.height <= 0.55f){
            controller.height = 0.2f;
        }
        currentHeight = controller.height;
    }

    void standing()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 2f)){
            if (hit.distance < 2f - 0.5f){
                controller.height = 0.5f;
                isCrouching = true;

            }else{
                controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime * standingSpeed);
                if (controller.height >= 1.95f){
                    controller.height = 2f;
                }
                currentHeight = controller.height;
            }

        }else{
            controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime * standingSpeed);
            if (controller.height >= 1.905){
                controller.height = 2f;

            }
            currentHeight = controller.height;
        }
    }
}