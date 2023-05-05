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
                if (Input.GetKey(KeyCode.Mouse0)) shootHolder();
                break;

            //If it's semi auto, call every time we click
            case gunSettingsScriptable.gunType.semiAuto:
                if (Input.GetKeyDown(KeyCode.Mouse0)) shootHolder();
                break;

            //how tf can I get this case?
            default:
                Debug.LogError("bro what the fuck how did this happen");
                break;
        }

        if (gunMag < 1 && totalAmmo != 0 && canReload == true && anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE)) reload = StartCoroutine(IwaitFFS());

        void shootHolder()
        { 
            if (gunMag != 0)
            {
                if (!interactCont.hitSomething && (anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE) || anim.GetCurrentAnimatorStateInfo(0).IsName(RELOADSTATE)))
                {
                    cameraMove.mouseYRestrict = settings.mouseYrestrict;
                    cameraMove.mouseXRestrict = settings.mouseXrestrict;

                    if (reload != null)
                    {
                        StopCoroutine(reload);
                        canReload = true;
                    }

                    Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);

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
