using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorInteractController : Interactable
{
    [SerializeField] private interactDoor Door;
    [SerializeField] private Transform playerLocation;


    public override string getDescription()
    {
        if(Door.isMoving && !Door.isOpen){
            return "Opening Door";
        }
        else if(Door.isMoving && Door.isOpen){
            return "Closing Door";
        }
        else if (Door.isOpen){
            return "Close Door";
        }
        else{
            return "Open Door";
        }
    }

    public override void interact()
    {
        if (Door.isOpen && !Door.isMoving)
        {
            Door.close();
        }
        else if(!Door.isOpen && !Door.isMoving)
        {
            Door.open();
        }
    }
}
