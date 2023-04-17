using UnityEngine;

public abstract class gunSettingsScriptable : ScriptableObject
{
    [Header("Gun Settings")]
    public string gunName;
    public GameObject bulletType;
    public float damage;

    [Header("Bullet Settings")]
    public float bulletSpeed;
    public float bulletDistance;

    [Header("Gun Recoil Settings")]
    public float recoilTargetRot;
    public float recoilTargetPush;
    public float maxPosRecoil;
    public float rotRecoilReturnSpeed;
    public float posRecoilReturnSpeed;

    [Header("Camera Recoil Settings")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float recoilReturn;
    public float snappiness;

    [Header("Reload")]
    public int ammoMag;
    public int ammoCount;

    [Header("Gun Bob")]
    public float frequency;
    public float amplitude;

    [Header("Can Aim")]
    public bool canAim;
    public float aimMultiplier;

    [Header("Dynamic Crosshair")]
    public float sittingCross;
    public float movingCross;
    public float crossSpeed;
    public float crossSizeIncrease;

    [Header("Gun Type")]
    public gunType guntype;

    public enum gunType
    {
        fullAuto,
        semiAuto,
    }

    public abstract void onFireFunc(Ray ray, RaycastHit hit, Transform shootArea);
}


/*
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
}*/