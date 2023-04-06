using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gunSettings", menuName = "scriptables")]
public class pistol : gunSettingsScriptable
{
    public override void onFire(Ray ray, RaycastHit hit, Transform shootArea)
    {
        GameObject bullet = Instantiate(bulletType, shootArea.transform.position, Quaternion.identity);

        //If we hit something
        if (Physics.Raycast(ray, out hit, bulletDistance))
        {
            shootBullet(bullet, hit.point);
            //If we miss and hit nothing
        }
        else
        {
            shootBullet(bullet, ray.GetPoint(bulletDistance));
        }

        void shootBullet(GameObject bullet, Vector3 hitPoint)
        {
            //get the distance we want to travel by subtracting the place we hit with where our bullet will be shot
            Vector3 travelDistance = hitPoint - shootArea.position;

            //add a force to the bullet using the direction our bullet hit multiplied by the bullet speed and using the Impulse mode in order to make sure to add an instant force
            bullet.GetComponent<Rigidbody>().AddForce(travelDistance.normalized * bulletSpeed, ForceMode.Impulse);
        }
    }
}
