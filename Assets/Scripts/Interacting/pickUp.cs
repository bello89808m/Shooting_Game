using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] public Transform placeArea;

    public Transform getTransformArea() => placeArea;

    public string getDesc() => "Pick up " + this.name;
}
