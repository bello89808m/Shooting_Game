using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    public bool getDownTime();
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

public interface IFunction
{
    public void doThis();
}