using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class standardGunShoot : GunClass
{
    public override void shootFunc()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE))
                shootHolderFunc();

        //If our ammo is less than 1, our total ammo is not 0, and the animation is in its equipped state, start the reload coroutine
        if (gunMag < 1 && totalAmmo != 0 && canReload == true && anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE)) coReload = StartCoroutine(IwaitFFS());

        void shootHolderFunc()
        {
            //If the gunmag isn't 0
            if (gunMag != 0)
            {
                //If we aren't interacting with somethng, have the animation at the equipped state or has it in the reload state
                if (!interactCont.hitSomething)
                {
                    //Restrict the camera
                    cameraMove.mouseYRestrict = settings.mouseYrestrict;
                    cameraMove.mouseXRestrict = settings.mouseXrestrict;
                    //Launch a ray
                    Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);

                    //Run the shoot script that spawns the bullet. Rest is self explanatory
                    fire(ray, hit, shootArea);
                    recoilFunc();
                    gunMag--;
                    crossSize += settings.crossSizeIncrease;
                    anim.Play(SHOOTSTATE);
                }
            }
        }

        void fire(Ray ray, RaycastHit hit, Transform shootArea)
        {
            GameObject bullet = Instantiate(settings.bulletType, shootArea.transform.position, Quaternion.identity);

            //If we hit  something
            if (Physics.Raycast(ray, out hit, settings.bulletDistance, ~settings.mask))
            {
                shootBullet(bullet, hit.point);
                //If we miss and hit nothing
            } else {
                shootBullet(bullet, ray.GetPoint(settings.bulletDistance));
            }

            void shootBullet(GameObject bullet, Vector3 hitPoint)
            {
                //get the distance we want to travel by subtracting the place we hit with where our bullet will be shot
                Vector3 travelDistance = hitPoint - shootArea.position;

                //add a force to the bullet using the direction our bullet hit multiplied by the bullet speed and using the Impulse mode in order to make sure to add an instant force
                bullet.GetComponent<Rigidbody>().AddForce(travelDistance.normalized * settings.bulletSpeed, ForceMode.Impulse);
            }
        }
    }
}
