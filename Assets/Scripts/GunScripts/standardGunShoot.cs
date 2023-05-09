using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class standardGunShoot : GunClass
{
    public override void shootFunc()
    {
        //Check what kind of firing mode we have
        switch (settings.guntype)
        {
            //If its full auto, call the shoot script when we're holding the mouse down
            case gunSettingsScriptable.gunType.fullAuto:
                if (Input.GetKey(KeyCode.Mouse0)) shootHolderFunc();
                break;

            //If it's semi auto, call every time we click
            case gunSettingsScriptable.gunType.semiAuto:
                if (Input.GetKeyDown(KeyCode.Mouse0)) shootHolderFunc();
                break;

            //how tf can I get this case?
            default:
                Debug.LogError("bro what the fuck how did this happen");
                break;
        }

        //If our ammo is less than 1, our total ammo is not 0, and the animation is in its equipped state, start the reload coroutine
        if (gunMag < 1 && totalAmmo != 0 && canReload == true && anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE)) coReload = StartCoroutine(IwaitFFS());

        void shootHolderFunc()
        {
            //If the gunmag isn't 0
            if (gunMag != 0)
            {
                //If we aren't interacting with somethng, have the animation at the equipped state or has it in the reload state
                if (!interactCont.hitSomething && (anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE) || anim.GetCurrentAnimatorStateInfo(0).IsName(RELOADSTATE)))
                {
                    //Restrict the camera
                    cameraMove.mouseYRestrict = settings.mouseYrestrict;
                    cameraMove.mouseXRestrict = settings.mouseXrestrict;

                    //If we are relaoding, stop the reload coroutine 
                    if (coReload != null)
                    {
                        StopCoroutine(coReload);
                        canReload = true;
                    }

                    //Launch a ray
                    Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);

                    //Run the shoot script that spawns the bullet. Rest is self explanatory
                    settings.onFireFunc(ray, hit, shootArea);
                    recoilFunc();
                    gunMag--;
                    crossSize += settings.crossSizeIncrease;
                    anim.Play(SHOOTSTATE);
                }
            }
        }
    }
}
