using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using player;

public class gunShoot : MonoBehaviour, IPick, IFunction
{
    [Header("Gun Transforms")]
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform shootArea;

    [Header("Gun Settings")]
    [SerializeField] private gunSettingsScriptable settings;

    [Header("Player Cam")]
    [SerializeField] private Camera cam;

    [Header("Ammo Count")]
    [SerializeField] private TextMeshProUGUI ammoCount;

    [Header("Can Interact")]
    [SerializeField] private interactController interactCont;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    //Gun Down Time
    private float lastShootTime;

    //Raycast
    private RaycastHit hit;

    //Cam Recoil
    private Vector3 camTargetRot;
    private Vector3 camCurrentRot;

    //Gun Bob
    private Vector3 gunBobPos = Vector3.zero;

    //Ammo
    public int gunMag { get; private set; }
    public int totalAmmo { get; private set; }

    //Reload
    public bool reloading { get; private set; }
    public static bool showAmmo;
    public static string ammo;

    //IPick settings
    public string getDesc() => "Pick up " + settings.gunName;
    public Transform getTransformArea() => holdArea;

    //IFunction settings
    public void doThis()
    {
        shootType();
    }

    //****************************************************************************************************************************************************************************************************************************

    void Awake() {
        gunMag = settings.ammoMag;
        totalAmmo = settings.ammoCount;
    }

    //****************************************************************************************************************************************************************************************************************************

    void Update()
    {
        ammoCount.SetText(ammo);
        ammoCount.enabled = showAmmo;
        //Check if the gun is actually held first
        if (transform.parent == holdArea)
        {
            gunMoveController();
            recoilController();

        } else {
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    //Control how the gun moves
    void gunMoveController()
    {
        gunSway();
        //gunBob();
    }

    //****************************************************************************************************************************************************************************************************************************

    void gunSway()
    {
        //Get the axis of us turning the mouse and multiply it by how powerful we want the sway to be
        float x = Input.GetAxisRaw("Mouse X") * 750;
        float y = Input.GetAxisRaw("Mouse Y") * 800;

        //turn the gun on the direction our mouse is going by getting the sway and rotating it around the corresponding axis
        Quaternion xSway = Quaternion.AngleAxis(-x, Vector3.up);
        Quaternion ySway = Quaternion.AngleAxis(y, Vector3.right);

        //change the actual rotation of the gun itself
        transform.localRotation = Quaternion.Slerp(transform.localRotation, xSway * ySway, Time.deltaTime * 7);
    }

    //****************************************************************************************************************************************************************************************************************************

    /*void gunBob()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float swayMultiplier = frequencyMultiplier();

            gunBobPos.x += Mathf.Cos(Time.time * (0.4f/2 * swayMultiplier)) * 3.75f/2;
            gunBobPos.y += Mathf.Sin(Time.time * (0.4f * swayMultiplier)) * 3.75f/7.5f;
            gunBobPos.z = transform.localPosition.z;

            transform.localPosition = gunBobPos;

        } 
        resetBob();
    
    }*/

    //****************************************************************************************************************************************************************************************************************************

    void resetBob()
    {
        Vector3 startPos = new Vector3(0, 0, transform.localPosition.z);

        if (transform.localPosition != startPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 3f);
            gunBobPos = Vector3.zero;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    float frequencyMultiplier()
    {
        //Control the frequency of the gunbob aka how fast it should bob depending on the players movestate
        var moveState = FindObjectOfType<playerMovement>();

        switch (moveState.state)
        {
            case playerMovement.movementState.sprinting:
                return 2;
            case playerMovement.movementState.crouching:
                return 0.8f;
            default:
                return 1.4f;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void shootType()
    {
        //Check what kind of firing mode we have
        switch (settings.guntype)
        {
            //If its full auto, call the shoot script when we're holding the mouse down
            case gunSettingsScriptable.gunType.fullAuto:
                if(Input.GetKey(KeyCode.Mouse0)){
                    shootHolder();
                }

                break;

            //If it's semi auto, call every time we click
            case gunSettingsScriptable.gunType.semiAuto:
                if (Input.GetKeyDown(KeyCode.Mouse0)){
                    shootHolder();
                }

                break;

            //how tf can I get this case?
            default:
                Debug.LogError("bro what the fuck how did this happen");
                break;
        }



        if (Input.GetKeyDown(KeyCode.R) && totalAmmo != 0 && gunMag != settings.ammoMag) reload();

        void shootHolder()
        {
            if (gunMag == 0 && totalAmmo != 0) reload();
            else if (!(gunMag == 0)) shoot();
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void reload()
    {
        StartCoroutine(waitFFS());
    }

    //****************************************************************************************************************************************************************************************************************************

    IEnumerator waitFFS()
    {
        reloading = true;
        yield return new WaitForSeconds(1f);

        int reloadAmount = settings.ammoMag - gunMag;

        if(reloadAmount > totalAmmo)
        {
            gunMag += totalAmmo;
            totalAmmo = 0;

        } else {
            gunMag += reloadAmount;
            totalAmmo -= reloadAmount;
        }

        reloading = false;
    }

    //****************************************************************************************************************************************************************************************************************************

    void shoot()
    {
        //Check if the last time we shot along with the delay is less than the current time in order to not fire all our bullets at once
        if (lastShootTime + settings.shootDelay < Time.time)
        {
            //launch a ray from the center of the screen
            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
            //create a bullet from the bullet prefab
            gunMag--;
            recoil();

            settings.onFire(ray, hit, shootArea);

            //Set the last time we shot
            lastShootTime = Time.time;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void recoilController()
    {
        //Lerp from where our camera is rotoated to zero constantly
        camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, Time.deltaTime * settings.recoilReturn);
        //Slerp the cam currentRot to target rot in order to add a bit of delay. However this can also function with the TargetRot, this one is a bit more for the aethestics
        camCurrentRot = Vector3.Slerp(camCurrentRot, camTargetRot, Time.fixedDeltaTime * settings.snappiness);
        //make our local rotation the currentRot
        cam.transform.localRotation = Quaternion.Euler(camCurrentRot);

        //Move our local position to zero but do not mess with the x and y values in order to avoid clashes in code
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, 0), Time.deltaTime * settings.posRecoilReturnSpeed);
        //Make our rotation always slerp to 0
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * settings.rotRecoilReturnSpeed);
    }

    //****************************************************************************************************************************************************************************************************************************

    void recoil()
    {
        //Make the cam target rot recoil up a certain amount and add some random recoil in the ranges of the -Y and the poitive Y, same for the Z axis
        camTargetRot += new Vector3(-settings.recoilX, Random.Range(-settings.recoilY, settings.recoilY), Random.Range(-settings.recoilZ, settings.recoilZ));

        //Rotate our gun as far back as we want it, depending on the settings
        transform.localRotation = Quaternion.Euler(-settings.recoilTargetRot, 0, 0);

        //Get the local position of our gun
        Vector3 posRecoil = transform.localPosition;
        //Subtract the position by the target push backwars
        posRecoil -= Vector3.forward * settings.recoilTargetPush;
        //Clamp it to a specified amount in order to avoid a super far back recoil
        posRecoil.z = Mathf.Clamp(posRecoil.z,-settings.maxPosRecoil,0);
        //Set the position to be the recoil. Can this be more efficient? Yes. Did this bull shit take me so long even though it's the easiest thing ever and is 100% not what humbled my ego in this project? Yes. So will I fix it? Idk.
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