using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
}

public interface IPlace
{
    public string getDescription(GameObject name);

    public Transform getPlaceArea();
}

public interface IPick
{
    public string getDesc();

    public Transform getTransformArea();
}