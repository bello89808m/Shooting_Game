using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerDoor : MonoBehaviour
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
                door.Play(doorOpen,0,0.0f);
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if(closeTrigger)
        {
            door.Play(doorClose,0,0.0f);
            gameObject.SetActive(false);
        }
    }
}
