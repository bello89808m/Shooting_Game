using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] new Camera camera;
    [SerializeField] private float sprintFOV;
    [SerializeField] private float walkFOV;
    private float FOV;
    private float x, z;
    public bool isSprinting;

    [Header("Gravity")]
    private Vector3 velocity;
    private float gravity = -5f;

    [Header("Crouch")]
    [SerializeField] private float crouchingSpeed;
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
            velocity.y = -3.5f;
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

            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, walkFOV, 10 * Time.deltaTime);
        }
        else if(isSprinting && (x != 0 || z != 0)){
            state = movementState.sprinting;
            speed = 4f;

            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, sprintFOV, 10 * Time.deltaTime);
        }
        else if(x != 0 || z != 0){
            state = movementState.walking;
            speed = 3f;

            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, walkFOV, 10 * Time.deltaTime);
        }
        else{
            state = movementState.still;
            speed = 0f;

            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, walkFOV, 10 * Time.deltaTime);
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
                controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime * crouchingSpeed);
                if (controller.height >= 1.95f){
                    controller.height = 2f;
                }
                currentHeight = controller.height;
            }

        }else{
            controller.height = Mathf.Lerp(currentHeight, 2f, Time.deltaTime * crouchingSpeed);
            if (controller.height >= 1.905){
                controller.height = 2f;

            }
            currentHeight = controller.height;
        }
    }
}
