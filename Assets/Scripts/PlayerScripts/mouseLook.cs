using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace player
{
    public class mouseLook : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 7500f;
        private float xRotation = 0f;

        [SerializeField] private GameObject cam;
        [SerializeField] private Transform playerBody;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //**************************************************************************************************************

        void Update()
        {
            MouseLook();
        }

        //**************************************************************************************************************

        void MouseLook()
        {
            //looking left and right
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            //looking up and down
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

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
