using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //what Type
    public enum InteractionType
    {
        Click,
        Hold
    }

    public InteractionType interactiontype;

    //Hold interacts
    public float holdTime;
    public void holdingTime() => holdTime += Time.deltaTime;
    public void resetTime() => holdTime = 0;
    public float getHoldTime() => holdTime;

    //turn on/off
    public bool canInteract = true;

    //fundamentals
    public abstract void interact();
    public abstract string getDescription();
    public abstract KeyCode definedKey();
}

//Might be useful later
/*public interface IHold
{
    public string getDescription();

    public Rigidbody getObjBody();

    public Transform getTransformArea();
}*/
