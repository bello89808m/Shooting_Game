using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class pickUp2 : MonoBehaviour, IHold
{
    [SerializeField] private Rigidbody objBody;

    public string getDescription() => "Pick Up " + this.name;

    public void onInteract()
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;

        gameObject.layer = 6;

        objBody.isKinematic = true;
        objBody.detectCollisions = true;
    }    
}
