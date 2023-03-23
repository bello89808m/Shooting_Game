using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using player;

public class destroyBullet : MonoBehaviour
{
    private void Update()
    {
        Destroy(gameObject, 5f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        var getGun = FindObjectOfType<interactController>().pickUpObj;
        if (collision.gameObject == getGun) return;
        Destroy(gameObject);
    }
}
