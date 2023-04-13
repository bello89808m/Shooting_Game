using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorInteractController : MonoBehaviour, Interactable
{
    [SerializeField] private interactDoor Door;
    [SerializeField] private Transform playerLocation;


    public string getDescriptionFunc()
    {
        if (Door.isOpen){
            return "[E] Close Door";
            
        }else{
            return "[E] Open Door";
        }
    }

    public bool getDownTimeFunc()
    {
        if (Door.isMoving) return true;
        else return false;
    }

    public void interactFunc()
    {
        if (Door.isOpen && !Door.isMoving)
        {
            Door.closeFunc();

        }else if(!Door.isOpen && !Door.isMoving){
            Door.openFunc();
        }
    }

    Interactable.InteractionType Interactable.getTypeFunc()
    {
        return Interactable.InteractionType.Click;
    }
}
