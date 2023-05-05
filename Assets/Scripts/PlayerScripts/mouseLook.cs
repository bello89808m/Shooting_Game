using UnityEngine;

namespace player
{
    public class mouseLook : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 7500f;
        private float xRotation = 0f;

        [SerializeField] private GameObject cam;
        [SerializeField] private Transform playerBody;

        public float mouseXRestrict;
        public float mouseYRestrict;

        private interactController interactCont;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            interactCont = FindObjectOfType<interactController>();
            mouseXRestrict = 1;
            mouseYRestrict = 1;
        }

        //**************************************************************************************************************

        void Update()
        {
            MouseLookFunc();
        }

        //**************************************************************************************************************

        void MouseLookFunc()
        {
            if(interactCont.pickUpObj == null) cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.identity, Time.deltaTime * 4);
            //looking left and right
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * mouseXRestrict * Time.deltaTime;
            //looking up and down
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * mouseYRestrict * Time.deltaTime;

            //Get the negative of mouseY because without it the thing flips
            xRotation -= mouseY;
            //Make sure we cant look over
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            //Rotate our character left to right
            playerBody.Rotate(Vector3.up * mouseX);
            //Rotate the camera up and down
            transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.y, transform.localRotation.z);
        }
    }
}
