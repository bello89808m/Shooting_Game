using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class standardGunShoot : GunClass
{
    public override void shootFunc()
    {
        //Check if the last time we shot along with the delay is less than the current time in order to not fire all our bullets at once
        if (!interactCont.hitSomething &&
            (anim.GetCurrentAnimatorStateInfo(0).IsName("equipped") || anim.GetCurrentAnimatorStateInfo(0).IsName("reload")))
        {
            cameraMove.mouseYRestrict = settings.mouseYrestrict;
            cameraMove.mouseXRestrict = settings.mouseXrestrict;

            if (reload != null) { 
                StopCoroutine(reload);
                canReload = true;
            }

            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);

            settings.onFireFunc(ray, hit, shootArea);
            recoilFunc();
            gunMag--;
            crossSize += settings.crossSizeIncrease;
            anim.Play("shoot");
        }
    }
}
