using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        Click,
        Hold
    }

    public InteractionType interactiontype;

    public float holdTime;
    public void holdingTime() => holdTime += Time.deltaTime;
    public void resetTime() => holdTime = 0;
    public float getHoldTime() => holdTime;

    public bool canInteract = true;

    public abstract void interact();
    public abstract string getDescription();
}
