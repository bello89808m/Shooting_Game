using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : Interactable
{
    [SerializeField] private GameObject sphere;
    [SerializeField] private Transform parent;
    public override string getDescription()
    {
        return "Spawn ball";
    }

    public override void interact()
    {
        GameObject Newsphere = Instantiate(sphere, new Vector3(Random.Range(0, 10), 1, Random.Range(0, 1)), Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)), parent);
        Rigidbody sphereBody = Newsphere.GetComponent<Rigidbody>();
        sphereBody.velocity = Vector3.up * 2500 * Time.deltaTime;
    }
}
