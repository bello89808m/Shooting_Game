using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyBullet : MonoBehaviour
{
    private void Update()
    {
        Destroy(gameObject, 5f);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<gunShoot>()) return;
        Destroy(gameObject);
    }
}
