using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Holding : MonoBehaviour
{
    public enum holdType
    {
        pickUp,
        throwing,
        placing,

    }
    public holdType HoldType;

    public abstract string getDescription();

    public bool occupied;
}

public interface ICustom
{
    public void Interact();
    public KeyCode definedKey();
}
