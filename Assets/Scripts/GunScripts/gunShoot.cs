using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Threading;
using player;

public class gunShoot : MonoBehaviour, IPick
{
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform shootArea;
    [SerializeField] private Camera cam;
    [SerializeField] private gunScriptable settings;
    [SerializeField] private RectTransform dynamicCrosshair;

    private float lastShootTime;

    private RaycastHit hit;

    private Vector3 camTargetRot;
    private Vector3 camCurrentRot;

    private Vector3 startPos;

    private Vector3 gunBobPos = Vector3.zero;

    public string getDesc() => "Pick up " + settings.gunName;

    public Transform getTransformArea() => holdArea;

    void Awake() => startPos = transform.localPosition;

    void Update()
    {
        if (transform.parent == holdArea)
        {
            gunMoveController();
            shootType();
        }
    }

    void gunMoveController()
    {
        gunSway();
        gunBob();
    }

    void gunSway()
    {
        float x = Input.GetAxisRaw("Mouse X") * settings.swayMultiplier;
        float y = Input.GetAxisRaw("Mouse Y") * settings.swayMultiplier;

        Quaternion xSway = Quaternion.AngleAxis(-x, Vector3.up);
        Quaternion ySway = Quaternion.AngleAxis(y, Vector3.right);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, xSway * ySway, Time.deltaTime * settings.swaySmooth);
    }

    void gunBob()
    {
        if (Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical") != 0)
        {
            float swayMultiplier = frequencyMultiplier();

            gunBobPos.x += Mathf.Cos(Time.time * (settings.frequency/2 * swayMultiplier)) * settings.amplitude/2;
            gunBobPos.y += Mathf.Sin(Time.time * (settings.frequency * swayMultiplier)) * settings.amplitude;
            gunBobPos.z = transform.localPosition.z;

            transform.localPosition = gunBobPos;
        }
        resetBob();
    }

    void resetBob()
    {
        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, transform.localPosition.z), Time.deltaTime * 5f);
            gunBobPos = Vector3.zero;
        }
    }

    float frequencyMultiplier()
    {
        var moveState = FindObjectOfType<playerMovement>();

        switch (moveState.state)
        {
            case playerMovement.movementState.sprinting:
                return 2;
            case playerMovement.movementState.crouching:
                return 0.85f;
            default:
                return 1;
        }
    }

    void shootType()
    {
        recoilController();
        switch (settings.guntype)
        {
            case gunScriptable.gunType.fullAuto:

                if (Input.GetKey(KeyCode.Mouse0)) shoot();
                break;

            case gunScriptable.gunType.semiAuto:
                if (Input.GetKeyDown(KeyCode.Mouse0)) shoot();
                break;

            default:
                Debug.LogError("bro what the fuck how did this happen");
                break;
        }
    }

    void shoot()
    {
        if (lastShootTime + settings.shootDelay < Time.time)
        {
            recoil();

            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
            GameObject bullet = Instantiate(settings.bullet, shootArea.transform.position, Quaternion.identity);

            if (Physics.Raycast(ray, out hit, settings.bulletDistance)){
                shootBullet(bullet, hit.point);
            }else{
                shootBullet(bullet, ray.GetPoint(settings.bulletDistance));
            }

            lastShootTime = Time.time;
        }
    }

    void shootBullet(GameObject bullet, Vector3 hitPoint)
    {
        Vector3 travelDistance = hitPoint - shootArea.position;
        bullet.GetComponent<Rigidbody>().AddForce(travelDistance.normalized * settings.bulletSpeed, ForceMode.Impulse);
    }

    void recoilController()
    {
        camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, Time.deltaTime * settings.recoilReturn);
        camCurrentRot = Vector3.Slerp(camCurrentRot, camTargetRot, Time.fixedDeltaTime * settings.snappiness);
        cam.transform.localRotation = Quaternion.Euler(camTargetRot);

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, 0), Time.deltaTime * settings.posRecoilReturnSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * settings.rotRecoilReturnSpeed);
    }

    void recoil()
    {
        camTargetRot += new Vector3(-settings.recoilX, Random.Range(-settings.recoilY, settings.recoilY), Random.Range(-settings.recoilZ, settings.recoilZ));

        transform.localRotation = Quaternion.Euler(-settings.recoilTargetRot, 0, 0);

        Vector3 posRecoil = transform.localPosition;
        posRecoil -= Vector3.forward * settings.recoilTargetPush;
        posRecoil.z = Mathf.Clamp(posRecoil.z,-settings.maxPosRecoil,0);
        transform.localPosition = posRecoil;
    }
}
















/*camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, Time.deltaTime * settings.recoilReturn);
        camCurrentRot = Vector3.Slerp(camCurrentRot, camTargetRot, Time.fixedDeltaTime * settings.snappiness);
        cam.transform.localRotation = Quaternion.Euler(camCurrentRot);/*


// camTargetRot += new Vector3(settings.recoilX, Random.Range(-settings.recoilY, settings.recoilY), Random.Range(-settings.recoilZ, settings.recoilZ));


//float swayX = Input.GetAxis("Mouse X") * -settings.swayMultiplier;
        float swayY = Input.GetAxis("Mouse Y") * settings.swayMultiplier;

        Quaternion rotRight = Quaternion.AngleAxis(swayX, Vector3.up);
        Quaternion rotUp = Quaternion.AngleAxis(swayY, Vector3.right);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotRight * rotUp, Time.deltaTime * settings.swaySmooth);





//You never know when you might wanna switch


/*IEnumerator shootTrail(TrailRenderer bullet, Vector3 distance)
    {
        Vector3 startPos = bullet.transform.position;
        float travelDistance = Vector3.Distance(shootArea.position, distance);
        float remainingDistance = travelDistance;
        Debug.Log("a");

        while(remainingDistance > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPos, distance, 1 - (remainingDistance / travelDistance));

            remainingDistance -= Time.deltaTime * settings.bulletSpeed;

            yield return null;
        }

        Destroy(bullet.gameObject, bullet.time);
    }*/