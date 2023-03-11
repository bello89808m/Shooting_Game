using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class place : MonoBehaviour
{
    [SerializeField] private Collider getCollider;
    [SerializeField] private string description;

    public string getDescription() => description;

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
