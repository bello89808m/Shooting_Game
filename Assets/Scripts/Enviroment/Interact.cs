using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : Interactable
{
    public override string getDescription()
    {
        return "[E] Pick Up";
    }

    public override void interact()
    {
        Debug.Log("working");
    }
}
