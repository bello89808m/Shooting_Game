using UnityEngine;

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
    public void resetValuesFunc();

    public string getDescFunc();

    public Transform getTransformAreaFunc();

    public Transform getRightHandTargetFunc();
    public Transform getLeftHandTargetFunc();

    public Transform getRightHandHintFunc();
    public Transform getLeftHandHintFunc();
}