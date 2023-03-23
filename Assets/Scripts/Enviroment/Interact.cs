using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour, Interactable
{
    public string getDescription()
    {
        return "[E] Pick Up";
    }

    public void interact()
    {
        Debug.Log("working");
    }

    Interactable.InteractionType Interactable.getType()
    {
        return Interactable.InteractionType.Hold;
    }
}
