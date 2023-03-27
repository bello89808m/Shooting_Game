using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour, Interactable
{
    private float downTime;
    public string getDescription()
    {
        return "[E] Pick Up";
    }

    public bool getDownTime()
    {
        if (downTime + 0.5f < Time.time) return false;
        else return true;
    }

    public void interact()
    {
        Debug.Log("working");
        downTime = Time.time;
    }

    Interactable.InteractionType Interactable.getType()
    {
        return Interactable.InteractionType.Hold;
    }
}
