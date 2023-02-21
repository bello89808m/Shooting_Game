using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class pickUp2 : MonoBehaviour, IHold
{
    public string getDescription()
    {
        return "[E] Pick Up Square 2";
    }
}
