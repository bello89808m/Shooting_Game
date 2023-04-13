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
    public void interactFunc();
    public bool getDownTimeFunc();
    public string getDescriptionFunc();
    public InteractionType getTypeFunc();
}

public interface IPlace
{
    public string getDescriptionFunc(GameObject name);

    public Transform getPlaceAreaFunc();
}

public interface IPick
{
    public string getDescFunc();

    public Transform getTransformAreaFunc();
}

public interface IFunction
{
    public void doThisFunc();
}