using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBob : MonoBehaviour
{
    [SerializeField] private Transform camera = null;
    [SerializeField] private Transform cameraHolder = null;

    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;

    private float minSpeed = 3.0f;
    private float maxSpeed = 5.0f;
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
        camera.LookAt(FocusTarget()); 
    }

    private void PlayMotion(Vector3 motion)
    {
        camera.localPosition += motion; 
    }

    private void CheckMotion()
    {
        float speed = new Vector3(controller.velocity.x, 0 , controller.velocity.z).magnitude;
        ResetPosition();

        if(minSpeed < speed && speed < maxSpeed){
            PlayMotion(FootSteps(10.0f, 0.002f));
        }else if(speed >= maxSpeed){
            PlayMotion(FootSteps(15.0f, 0.005f));
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

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }
}
