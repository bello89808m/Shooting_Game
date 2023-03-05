using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class pickUp : MonoBehaviour
{
    [SerializeField] private Transform placeArea;

    public string getDescription() => this.name;

    public Rigidbody getObjBody() => this.GetComponent<Rigidbody>();

    public Transform getTransformArea() => placeArea;
}
