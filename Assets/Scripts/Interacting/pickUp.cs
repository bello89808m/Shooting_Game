using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] private Transform placeArea;

    public Transform getTransformArea() => placeArea;
}
