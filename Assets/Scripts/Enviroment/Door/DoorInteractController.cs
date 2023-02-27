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
        if(Door.isMoving){
            return "";
        }
        else if (Door.isOpen){
            return "[E] Close Door";
        }
        else{
            return "[E] Open Door";
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

    public override KeyCode definedKey()
    {
        return KeyCode.E;
    }
}
