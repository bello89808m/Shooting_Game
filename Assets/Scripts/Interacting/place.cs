using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class place : MonoBehaviour, IPlace
{
    [SerializeField] private Collider getCollider;

    public string getDescription(GameObject name) => "Place " + name.name;

    public Transform getPlaceArea() => transform;

    private void Update()
    {
        if(transform.childCount > 0){

            GetComponent<Collider>().enabled = false;
        } else {
            GetComponent<Collider>().enabled = true;
        }
    }
}
