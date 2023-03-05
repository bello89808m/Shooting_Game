using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] new Camera camera;
    [SerializeField] Camera holdingCamera;
    private float sprintFOV = 75;
    private float walkFOV = 60;
    private float x, z;
    private bool isSprinting;

    [Header("Gravity")]
    private Vector3 velocity;
    private float gravity = -5f;

    [Header("Crouch")]
    [SerializeField] private float crouchingSpeed;
    private float currentHeight;
    private bool isCrouching;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

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
            velocity.y = -3.5f;
        }

        //Gravity

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

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
        float targetFOV = walkFOV;

        if(x == 0 && z == 0){
            state = movementState.still;
            speed = 0f;

        }else if (isCrouching){
            state = movementState.crouching;
            speed = 2f;

        }else if (isSprinting){
            state = movementState.sprinting;
            speed = 4f;

            targetFOV = sprintFOV;

        }else{
            state = movementState.walking;
            speed = 3f;
        }

        //Camera FOV
        float actualFOV = Mathf.MoveTowards(camera.fieldOfView, targetFOV, 50 * Time.deltaTime);

        camera.fieldOfView = actualFOV;
        holdingCamera.fieldOfView = actualFOV;
    }

    void crouching()
    {
        //Move Towards this height
        controller.height = Mathf.MoveTowards(currentHeight, 0.5f, Time.deltaTime * crouchingSpeed);
        if (controller.height <= 0.55f){
            controller.height = 0.2f;
        }
        currentHeight = controller.height;
    }

    //This needs to be fixed but I'm just too lazy to
    void standing()
    {
        //Check nothing is above us
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 2f)){
            //If something is above us
            if (hit.distance < 2f - 0.5f){
                controller.height = 0.5f;
                isCrouching = true;
            //If nothing is above us
            }else{
                controller.height = Mathf.MoveTowards(currentHeight, 2f, Time.deltaTime * crouchingSpeed);
                if (controller.height >= 1.95f){
                    controller.height = 2f;
                }
                currentHeight = controller.height;
            }
        //If the raycast hits nothing
        }else{
            controller.height = Mathf.MoveTowards(currentHeight, 2f, Time.deltaTime * crouchingSpeed);
            if (controller.height >= 1.905){
                controller.height = 2f;

            }
            currentHeight = controller.height;
        }
    }
}
