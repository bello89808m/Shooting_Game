using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float speed = 3f;
    public float sprintSpeed = 4f;
    private float x, z;
    public bool isSprinting;

    [Header("Gravity")]
    private Vector3 velocity;
    private float gravity = -5f;

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchingSpeed;
    public float currentHeight;
    private float elapsedTime;
    public bool isCrouching;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public bool isGrounded;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    void Update()
    {
        playerMove();
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

        //Crouching

        if(isCrouching)
        {
            crouching();
        }else{
            standing();
        }

        //Movement State

        if(isCrouching)
        {
            controller.Move(Vector3.ClampMagnitude(move,1.0f) * crouchSpeed * Time.deltaTime);
        }else if(isSprinting)
        {
            controller.Move(Vector3.ClampMagnitude(move,1.0f) * sprintSpeed * Time.deltaTime);
        }else
        {
            controller.Move(Vector3.ClampMagnitude(move,1.0f) * speed * Time.deltaTime);
        }

        void crouching(){
            controller.height = Mathf.Lerp(currentHeight, 0.5f, Time.deltaTime*crouchingSpeed);
            if(controller.height <= 0.55f){
                controller.height = 0.5f;
            }
            currentHeight = controller.height;
        }

        void standing(){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.up, out hit, 2f))
            {
                if(hit.distance < 2f - 0.5f){
                    controller.height = 0.5f;
                    isCrouching = true;
                }
                else{
                    controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime*crouchingSpeed);
                    if(controller.height >= 1.90f){
                        controller.height = 2f;
                    }
                    currentHeight = controller.height;
                }
            }
            else{
                    controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime*crouchingSpeed);
                    if(controller.height >= 1.90f){
                        controller.height = 2f;
                    }
                    currentHeight = controller.height;
                }
        }
    }
}
