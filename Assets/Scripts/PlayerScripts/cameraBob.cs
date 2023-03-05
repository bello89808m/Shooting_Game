using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBob : MonoBehaviour
{
    [SerializeField] private new Transform camera = null;
    [SerializeField] private playerMovement playerSpeed;
    [SerializeField] private CharacterController controller;

    private float amplitude;
    private float frequency;

    private Vector3 startPos;


    // Start is called before the first frame update
    private void Awake()
    {
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

        if ((playerSpeed.state == playerMovement.movementState.walking || playerSpeed.state == playerMovement.movementState.sprinting) && controller.height >= 2f) {
            PlayMotion(FootSteps(6f, 0.0025f));

        } else if(playerSpeed.state == playerMovement.movementState.crouching){
            PlayMotion(FootSteps(4.5f, 0.0015f));
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
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 5 * Time.deltaTime);
    }
}
