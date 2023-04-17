using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using player;
using System;

public class gunShoot : MonoBehaviour, IPick, IFunction
{
    [Header("Gun Transforms")]
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform aimArea;
    [SerializeField] private Transform shootArea;

    [Header("Gun Settings")]
    [SerializeField] private gunSettingsScriptable settings;

    [Header("Player Cam")]
    [SerializeField] private Camera cam;
    private RaycastHit hit;

    [Header("Ammo Count")]
    [SerializeField] private TextMeshProUGUI ammoCount;

    [Header("Crosshair")]
    [SerializeField] private GameObject cursor;
    [SerializeField] private RectTransform crossHair;
    public GameObject crossGetter { get; private set; }
    private float crossSize;

    [Header("Animator")]
    [SerializeField] private Animator anim;

    //interact controller
    private interactController interactCont;

    //Cam Recoil
    private Vector3 camTargetRot;
    private Vector3 camCurrentRot;

    //Gun Bob
    private Vector3 gunBobPos = Vector3.zero;

    //Ammo
    public int gunMag { get; private set; }
    public int totalAmmo { get; private set; }

    //Reload
    public static bool showAmmo;
    public static string ammo;

    //Aiming
    private bool isAiming;

    //IPick settings
    public string getDescFunc() => "Pick up " + settings.gunName;
    public Transform getTransformAreaFunc() => holdArea;

    //IFunction settings
    public void doThisFunc()
    {
        if(anim.isActiveAndEnabled && anim.GetCurrentAnimatorStateInfo(0).IsName("equipped") && !anim.IsInTransition(0)) shootContFunc();
    }

    public void resetValuesFunc()
    {

    }

    //****************************************************************************************************************************************************************************************************************************

    void Awake() {
        interactCont = FindObjectOfType<interactController>();

        gunMag = settings.ammoMag;
        totalAmmo = settings.ammoCount;

        crossGetter = crossHair.gameObject;
        crossHair.gameObject.SetActive(false);
    }

    //****************************************************************************************************************************************************************************************************************************

    void Update()
    {
        ammoCount.SetText(ammo);
        ammoCount.enabled = showAmmo;
        //Check if the gun is actually held first

        if (transform.parent == holdArea || transform.parent == aimArea)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle")) anim.Play("pickUp");
            else if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play("equipped");

            gunMoveControllerFunc();
            recoilControllerFunc();
            dynamicCrosshairMoveFunc();

        } else {
            anim.Play("idle");
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void dynamicCrosshairMoveFunc()
    {
        moveDynamicCrossChangeFunc();

        crossHair.sizeDelta = new Vector2(crossSize, crossSize);
    }

    //****************************************************************************************************************************************************************************************************************************

    void moveDynamicCrossChangeFunc()
    {
        if(Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical") > 0)
        {
            crossSize = Mathf.Lerp(crossSize, settings.movingCross, Time.deltaTime * settings.crossSpeed);
        } else {
            crossSize = Mathf.Lerp(crossSize, settings.sittingCross, Time.deltaTime * settings.crossSpeed);
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    //Control how the gun moves
    void gunMoveControllerFunc()
    {
        if (isAiming) return;
        gunSwayFunc();
        gunBobFunc();
    }

    //****************************************************************************************************************************************************************************************************************************

    void gunSwayFunc()
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

    void gunBobFunc()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float swayMultiplier = frequencyMultiplierFunc();

            gunBobPos.x += Mathf.Cos(Time.time * (settings.frequency/2 * swayMultiplier)) * settings.amplitude/2;
            gunBobPos.y += Mathf.Sin(Time.time * (settings.frequency * swayMultiplier)) * settings.amplitude/6.5f;
            gunBobPos.z = transform.localPosition.z;

            transform.localPosition = gunBobPos;

        } 
        resetBobFunc();
    
    }

    //****************************************************************************************************************************************************************************************************************************

    void resetBobFunc()
    {
        Vector3 startPos = new Vector3(0, 0, transform.localPosition.z);

        if (transform.localPosition != startPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 3f);
            gunBobPos = Vector3.zero;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    float frequencyMultiplierFunc()
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

    void shootContFunc()
    {
        var cameraMove = FindObjectOfType<mouseLook>();
        if(!Input.GetKey(KeyCode.Mouse0)) cameraMove.mouseRestrict = 1;

        //Check what kind of firing mode we have
        switch (settings.guntype)
        {
            //If its full auto, call the shoot script when we're holding the mouse down
            case gunSettingsScriptable.gunType.fullAuto:
                if(Input.GetKey(KeyCode.Mouse0)){

                    cameraMove.mouseRestrict = 0.15f;
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



        if (Input.GetKeyDown(KeyCode.R) && totalAmmo != 0 && gunMag != settings.ammoMag) StartCoroutine(IwaitFFS());

        if (Input.GetKey(KeyCode.Mouse1) && settings.canAim) aimingFunc();
        else if (isAiming && !Input.GetKey(KeyCode.Mouse1)) stopAimingFunc();

        void shootHolder()
        {
            if (gunMag == 0 && totalAmmo != 0) StartCoroutine(IwaitFFS());
            else if (!(gunMag == 0)) shootFunc();
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void aimingFunc()
    {
        isAiming = true;

        transform.SetParent(aimArea);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);

        cursor.SetActive(false);
    }

    void stopAimingFunc()
    {
        cursor.SetActive(true);

        transform.SetParent(holdArea);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);

        if(transform.localPosition == Vector3.zero) isAiming = false;
    }

    //****************************************************************************************************************************************************************************************************************************

    IEnumerator IwaitFFS()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        anim.Play("reload");

        yield return new WaitForSeconds(clips[1].length);

        int reloadAmount = settings.ammoMag - gunMag;

        if(reloadAmount > totalAmmo)
        {
            gunMag += totalAmmo;
            totalAmmo = 0;

        } else {
            gunMag += reloadAmount;
            totalAmmo -= reloadAmount;
        }

        anim.Play("equipped");
    }

    //****************************************************************************************************************************************************************************************************************************

    void shootFunc()
    {
        //Check if the last time we shot along with the delay is less than the current time in order to not fire all our bullets at once
        if (!interactCont.hitSomething)
        {
            //launch a ray from the center of the screen
            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
            //create a bullet from the bullet prefab
            recoilFunc();

            settings.onFireFunc(ray, hit, shootArea);

            gunMag--;

            crossSize -= settings.crossSizeIncrease;

            anim.Play("shoot");
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    void recoilControllerFunc()
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

    void recoilFunc()
    {
        float recoilMultiplier;

        if (isAiming) recoilMultiplier = settings.aimMultiplier;
        else recoilMultiplier = 1;

        //Make the cam target rot recoil up a certain amount and add some random recoil in the ranges of the -Y and the poitive Y, same for the Z axis
        camTargetRot += new Vector3(-settings.recoilX * recoilMultiplier, UnityEngine.Random.Range(-settings.recoilY, settings.recoilY) * recoilMultiplier, UnityEngine.Random.Range(-settings.recoilZ, settings.recoilZ) * recoilMultiplier);

        //Rotate our gun as far back as we want it, depending on the settings
        transform.localRotation = Quaternion.Euler(-settings.recoilTargetRot * recoilMultiplier, 0, 0);

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






//You never know when you might wanna switch


/*IEnumerator IshootTrail(TrailRenderer bullet, Vector3 distance)
    {
        Vector3 startPos = bullet.transform.position;
        float travelDistance = Vector3.Distance(shootArea.position, distance);
        float remainingDistance = travelDistance;

        while(remainingDistance > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPos, distance, 1 - (remainingDistance / travelDistance));

            remainingDistance -= Time.deltaTime * settings.bulletSpeed;

            yield return null;
        }

        Destroy(bullet.gameObject, bullet.time);
    }*/