using UnityEngine;

namespace player
{
    public class playerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        [Header("Movement")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform camHolder;
        [SerializeField] private float camRotTotal;
        [SerializeField] private float camRotSpeed;
        [SerializeField] private float sprintFOV = 75;
        [SerializeField] private float walkFOV = 1000000;
        public float speed = 4f;
        private float x, z, rawX;
        private bool isSprinting;


        [Header("Gravity")]
        private Vector3 velocity;
        private float gravity = -5f;

        [Header("GroundCheck")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance;
        private bool isGrounded;

        [Header("Keybinds")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

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

            rawX = Input.GetAxisRaw("Horizontal");

            //If we press shift we run
            isSprinting = Input.GetKey(sprintKey);

            //move the character depending on what we're pressing
            Vector3 move = transform.right * x + transform.forward * z;

            if(rawX < 0) camHolder.localRotation = Quaternion.Slerp(camHolder.localRotation, Quaternion.Euler(Vector3.forward * camRotTotal), Time.deltaTime * camRotSpeed);
            else if(rawX > 0) camHolder.localRotation = Quaternion.Slerp(camHolder.localRotation, Quaternion.Euler(Vector3.forward * -camRotTotal), Time.deltaTime * camRotSpeed);
            else camHolder.localRotation = Quaternion.Slerp(camHolder.localRotation, Quaternion.identity, Time.deltaTime * camRotSpeed);

            //Move the character and make sure the magnitude is not over one and then multiply it by it's speed
            controller.Move(Vector3.ClampMagnitude(move, 1.0f) * speed * Time.deltaTime);

            cameraFOVfunc();
        }

        //**************************************************************************************************************

        void groundedCheckFunc()
        {
            //Ground check
            //Create a sphere at the bottom of the player and see if theres something hitting it
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0) velocity.y = -3.5f;

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
                speed = 0;

            } else if (isSprinting) {
                state = movementState.sprinting;
                speed = 5.85f;

                targetFOV = sprintFOV;

            } else {
                state = movementState.walking;
                speed = 4.9f;
            }

            float actualFOV = Mathf.MoveTowards(camera.fieldOfView, targetFOV, 20 * Time.deltaTime);

            camera.fieldOfView = actualFOV;
        }
    }
}