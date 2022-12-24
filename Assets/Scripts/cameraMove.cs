using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    public Transform cameraPosition;
    public playerMovement pm;

    // Update is called once per frame
    private void Update()
    {
        if(pm.crouch)
        {
            transform.position = new Vector3(cameraPosition.position.x,0.15f,cameraPosition.position.z);
        }

        else
        {
            transform.position = cameraPosition.position;
        }
    }   
}
