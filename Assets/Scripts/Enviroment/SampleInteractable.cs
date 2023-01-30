using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleInteractable : Interactable
{
    public override string getDescription()
    {
        return "Interact";
    }

    public override void interact()
    {
        Debug.Log("Fuck You");
    }
}
