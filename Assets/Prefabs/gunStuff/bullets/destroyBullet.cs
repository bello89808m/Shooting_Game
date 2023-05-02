using UnityEngine;
using player;

public class destroyBullet : MonoBehaviour
{
    private void Update()
    {
        //destroy the bullet after 5 seconds
        Destroy(gameObject, 5f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        //If the thing we are hitting is not the gun itself, the bullet will kill itself NOW
        var getGun = FindObjectOfType<interactController>().pickUpObj;
        if (collision.gameObject == getGun || collision.transform.TryGetComponent(out destroyBullet bullet)) return;
        Destroy(gameObject);
    }
}
