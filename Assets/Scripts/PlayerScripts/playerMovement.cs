using UnityEngine;

namespace player
{
    public class playerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        [Header("Movement")]
        [SerializeField] new Camera camera;
        [SerializeField] Camera holdingCamera;
        [SerializeField] public float speed { get; private set; } = 3f;
        [SerializeField] private float sprintFOV = 65;
        [SerializeField] private float walkFOV = 60;
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

        public movementState state;
        //enum for how you're moving
        public enum movementState
        {
            walking,
            crouching,
            sprinting,
            still
        }

        //**************************************************************************************************************

        void Update()
        {
            playerMoveFunc();
        }

        //**************************************************************************************************************

        void playerMoveFunc()
        {
            groundedCheckFunc();

            //Moving the player
            //Get if we're pressing A or D 
            x = Input.GetAxis("Horizontal");
            //Get if we're pressing W or S
            z = Input.GetAxis("Vertical");

            //If we press shift we run
            isSprinting = Input.GetKey(sprintKey);
            //If we press control we run or whatever the fuck
            isCrouching = Input.GetKey(crouchKey);

            //move the character depending on what we're pressing
            Vector3 move = transform.right * x + transform.forward * z;

            //Move the character and make sure the magnitude is not over one and then multiply it by it's speed
            controller.Move(Vector3.ClampMagnitude(move, 1.0f) * speed * Time.deltaTime);

            //Crouching

            if (isCrouching)
            {
                crouchingFunc();
            } else {
                standingFunc();
            }

            cameraFOVfunc();
        }

        //**************************************************************************************************************

        //Who thought crouching would be so hard? Like seriously this wasted genuine days because I just couldn't think of a good solution and needed a tutorial to see how to change this. God damn you 2022 December me
        void crouchingFunc()
        {
            //Move Towards this height
            controller.height = Mathf.MoveTowards(currentHeight, 0.5f, Time.deltaTime * crouchingSpeed);
            if (controller.height <= 0.55f)
            {
                controller.height = 0.2f;
            }
            currentHeight = controller.height;
        }

        //**************************************************************************************************************

        //This probably needs to be fixed but I'm just too lazy to
        void standingFunc()
        {
            //Check nothing is above us
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, 3.65f))
            {
                //If something is above us
                if (hit.distance < 3.65f - 0.5f)
                {
                    controller.height = 0.5f;
                    isCrouching = true;
                    //If nothing is above us
                } else {
                    controller.height = Mathf.MoveTowards(currentHeight, 3.65f, Time.deltaTime * crouchingSpeed);
                    if (controller.height >= 1.95f) {
                        controller.height = 3.65f;
                    }
                    currentHeight = controller.height;
                }
                //If the raycast hits nothing
            } else {

                controller.height = Mathf.MoveTowards(currentHeight, 3.65f, Time.deltaTime * crouchingSpeed);

                if (controller.height >= 1.905) {
                    controller.height = 3.65f;

                }
                currentHeight = controller.height;
            }
        }

        //**************************************************************************************************************

        void groundedCheckFunc()
        {
            //Ground check
            //Create a sphere at the bottom of the player and see if theres something hitting it
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)velocity.y = -3.5f;

            //Gravity
            //idk the math here but add the current velocity of the character, -3.5, to the gravity and move it at a constant rate
            velocity.y += gravity * Time.deltaTime;
            //Move the player down depending on the velocity
            controller.Move(velocity * Time.deltaTime);
        }

        //**************************************************************************************************************

        void cameraFOVfunc()
        {
            //Movement State
            float targetFOV = walkFOV;

            //self explanatory
            if (x == 0 && z == 0)
            {
                state = movementState.still;
                speed = 0f;

            } else if (isCrouching) {
                state = movementState.crouching;
                speed = 2f;

            } else if (isSprinting) {
                state = movementState.sprinting;
                speed = 4f;

                targetFOV = sprintFOV;

            } else {
                state = movementState.walking;
                speed = 3f;
            }

            float actualFOV = Mathf.MoveTowards(camera.fieldOfView, targetFOV, 50 * Time.deltaTime);

            camera.fieldOfView = actualFOV;
            holdingCamera.fieldOfView = actualFOV;
        }
    }
}