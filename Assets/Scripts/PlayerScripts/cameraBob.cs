using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBob : MonoBehaviour
{
    [SerializeField] private Transform camera = null;
    [SerializeField] private playerMovement playerSpeed;

    private float amplitude;
    private float frequency;

    private Vector3 startPos;
    private CharacterController controller;


    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        startPos = camera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMotion();
    }

    private void PlayMotion(Vector3 motion)
    {
        camera.localPosition += motion; 
    }

    private void CheckMotion()
    { 
        ResetPosition();

        if (playerSpeed.state == playerMovement.movementState.walking){
            PlayMotion(FootSteps(8f, 0.002f));

        }else if(playerSpeed.state == playerMovement.movementState.sprinting){
            PlayMotion(FootSteps(9.5f, 0.003f));

        }else if (playerSpeed.state == playerMovement.movementState.crouching){
            PlayMotion(FootSteps(6f, 0.001f));
        }
    }

    private Vector3 FootSteps(float frequency, float amplitude)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        return pos;
    }

    private void ResetPosition()
    {   
        if(camera.localPosition == startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 7 * Time.deltaTime);
    }
}
