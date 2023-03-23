using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class InteractableInterfaces : MonoBehaviour
{
    //what Type
    public enum InteractionType
    {
        Click,
        Hold
    }

    public InteractionType interactiontype;

    //fundamentals
    public abstract void interact();
    public abstract string getDescription();
}

public interface Interactable
{
    //what Type
    public enum InteractionType
    {
        Click,
        Hold
    }

    //fundamentals
    public void interact();
    public string getDescription();
    public InteractionType getType();
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