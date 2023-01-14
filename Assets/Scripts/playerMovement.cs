using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float speed = 3f, sprintSpeed = 4f;
    float x, z;
    public bool isSprinting;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift, crouchKey = KeyCode.LeftControl;

    [Header("Crouch")]
    public float crouchSpeed, crouchingSpeed;
    public bool isCrouching, isVent;

    void Update()
    {
        playerMove();
    }

    void playerMove()
    {
        //Moving the player
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        isSprinting = Input.GetKey(sprintKey);
        isCrouching = Input.GetKey(crouchKey);

        if(isCrouching)
        {
            crouchMovement();
        }else
        {
            standingMovement();
        }

        Vector3 move = transform.right * x + transform.forward * z;

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
    }
    
    void crouchMovement()
    {
    }

    void standingMovement()
    {
    }
}
