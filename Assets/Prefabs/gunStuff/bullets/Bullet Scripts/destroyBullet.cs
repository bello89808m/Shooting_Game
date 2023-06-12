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
        //Get the interact controller script
        var getGun = FindObjectOfType<interactController>().pickUpObj;
        //If the thing we are hitting is not the gun itself or another bullet, the bullet will KILL itself NOW
        if (collision.gameObject == getGun || collision.transform.TryGetComponent(out destroyBullet bullet)) return;
        Destroy(gameObject);
    }
}
