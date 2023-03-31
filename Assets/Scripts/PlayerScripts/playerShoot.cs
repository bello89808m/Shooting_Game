using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace player
{
    public class playerShoot : MonoBehaviour
    {
        private interactController holdObj;

        private void Awake()
        {
            holdObj = FindObjectOfType<interactController>();
        }

        void Update()
        {
            if (holdObj.pickUpObj != null)
            {
                if (holdObj.pickUpObj.TryGetComponent<IFunction>(out IFunction functionThing) && functionThing.canFunc())
                {
                    functionThing.doThis();
                }

                if (holdObj.pickUpObj.TryGetComponent<gunShoot>(out gunShoot gun))
                {
                    gunShoot.showAmmo = true;
                    gunShoot.ammo = gun.gunMag.ToString() + "/" + gun.totalAmmo.ToString();
                    
                } else {
                    gunShoot.showAmmo = false;
                }
            } else {
                gunShoot.showAmmo = false;
            }
        }
    }
}