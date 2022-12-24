using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoor : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator door = null;

    [Header("Bools")]
    [SerializeField] bool closeTrigger = false;
    [SerializeField] bool openTrigger = false;

    [Header("Strings")]
    [SerializeField] string doorOpen = "Door Open";
    [SerializeField] string doorClose = "Door Close";
    
    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("player"))
        {
            if(openTrigger)
            {
                door.Play("Door Open",0,0.0f);
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if(closeTrigger)
        {
            door.Play("Door Close",0,0.0f);
            gameObject.SetActive(false);
        }
    }
}
