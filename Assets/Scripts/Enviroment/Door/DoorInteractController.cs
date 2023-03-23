using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorInteractController : MonoBehaviour, Interactable
{
    [SerializeField] private interactDoor Door;
    [SerializeField] private Transform playerLocation;


    public string getDescription()
    {
        if(Door.isMoving){
            return "";

        }else if (Door.isOpen){
            return "[E] Close Door";
            
        }else{
            return "[E] Open Door";
        }
    }

    public void interact()
    {
        if (Door.isOpen && !Door.isMoving)
        {
            Door.close();

        }else if(!Door.isOpen && !Door.isMoving){
            Door.open();
        }
    }

    Interactable.InteractionType Interactable.getType()
    {
        return Interactable.InteractionType.Click;
    }
}
