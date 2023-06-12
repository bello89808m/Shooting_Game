using System.Collections;
using System.Collections.Generic;
using player;
using TMPro;
using UnityEngine;

public abstract class GunClass : MonoBehaviour, IPick
{
    [Header("Gun Transforms")]
    [SerializeField] protected Transform shootArea;
    [SerializeField] protected Transform gunMoveTrans;
    [SerializeField] protected Transform gunRotTrans;
    protected Transform holdArea;

    [SerializeField] protected Transform leftHandArea;
    [SerializeField] protected Transform rightHandArea;

    [SerializeField] protected Transform leftHandHint;
    [SerializeField] protected Transform rightHandHint;

    [Header("Gun Settings")]
    [SerializeField] protected gunSettingsScriptable settings;

    [Header("Player Cam")]
    protected Camera cam;
    protected RaycastHit hit;

    [Header("Crosshair")]
    [SerializeField] protected RectTransform crossHair;
    public GameObject crossGetter { get; private set; }
    public static float crossSize;
    protected float crossSpeed;

    [Header("Animator")]
    [SerializeField] protected Animator anim;
    private string currentState;

    protected const string SHOOTSTATE = "shoot";
    protected const string IDLESTATE = "idle";
    protected const string EQUIPPEDSTATE = "equipped";
    protected const string PICKUPSTATE = "pickUp";
    protected const string RELOADSTATE = "reload";

    //Cam Recoil
    public static Vector3 camTargetRot;
    public static Quaternion camCurrentRot;
    private Vector3 currentPos;

    //Ammo
    public int gunMag;
    public int totalAmmo { get; private set; }

    //Reload
    public static bool showAmmo;
    public static string ammo;
    protected bool canReload = true;
    protected Coroutine coReload;

    //Aiming
    protected mouseLook cameraMove;
    protected interactController interactCont; 

    //Alt Fire
    protected bool altFire = false;

    //IPick settings
    public string getDescFunc() => "Pick up " + settings.gunName;
    public Transform getTransformAreaFunc() => holdArea;

    public Transform getRightHandTargetFunc() => rightHandArea;
    public Transform getLeftHandTargetFunc() => leftHandArea;

    public Transform getRightHandHintFunc() => rightHandHint;
    public Transform getLeftHandHintFunc() => leftHandHint;

    public void resetValuesFunc()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gunMoveTrans.localPosition = Vector3.zero;

        gunRotTrans.localPosition = Vector3.zero;
        gunRotTrans.localRotation = Quaternion.identity;

        canReload = true;

        cameraMove.mouseXRestrict = 1;
        cameraMove.mouseYRestrict = 1;

        crossSize = 0;

        if(coReload != null) StopCoroutine(coReload);
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void Awake()
    {
        //Get the mouselook script and the main camera
        cameraMove = FindObjectOfType<mouseLook>();
        interactCont = FindObjectOfType<interactController>();

        cam = Camera.main;
        gunMag = settings.ammoMag;
        totalAmmo = settings.ammoCount;

        crossGetter = crossHair.gameObject;
        crossHair.gameObject.SetActive(false);

        holdArea = GameObject.FindGameObjectWithTag("holdArea").transform;
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void Update()
    {
        //Check if the gun is actually held first

        if (transform.parent == holdArea)
        {
            anim.enabled = true ;

            shootContFunc();

            otherGunFunc();
            gunMoveControllerFunc();
            resetRecoilFunc();
            dynamicCrosshairMoveFunc();

        } else {

            anim.Play(PICKUPSTATE);
            anim.enabled = false;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void dynamicCrosshairMoveFunc()
    {
        moveDynamicCrossChangeFunc();

        crossHair.sizeDelta = new Vector2(crossSize, crossSize);
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void moveDynamicCrossChangeFunc()
    {
        if (Input.GetKey(KeyCode.Mouse0) && anim.GetCurrentAnimatorStateInfo(0).IsName(SHOOTSTATE))
        {
            crossSpeed = settings.crossSpeedDecrease;
        } else {
            crossSpeed = settings.crossSpeed;
        }

        crossSize = Mathf.Lerp(crossSize, settings.sittingCross, Time.deltaTime * crossSpeed);
        
    }

    //****************************************************************************************************************************************************************************************************************************

    //Control how the gun moves
    protected void gunMoveControllerFunc()
    {
        if (transform.parent != holdArea) return;
        gunSwayFunc();
        gunBobFunc();
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void gunSwayFunc()
    {
        //Get the axis of us turning the mouse and multiply it by how powerful we want the sway to be
        float x = Input.GetAxis("Mouse X") * 25;
        float z = Input.GetAxis("Mouse X") * 15f;
        float y = Input.GetAxis("Mouse Y") * 40;

        x = Mathf.Clamp(x, -25, 25);
        z = Mathf.Clamp(z, -15f, 15f);
        y = Mathf.Clamp(y, -40, 40);

        //turn the gun on the direction our mouse is going by getting the sway and rotating it around the corresponding axis
        Quaternion xSway = Quaternion.AngleAxis(-x, Vector3.up);
        Quaternion ySway = Quaternion.AngleAxis(y, Vector3.right);
        Quaternion zSway = Quaternion.AngleAxis(-z, Vector3.forward);

        //change the actual rotation of the gun itself
        gunRotTrans.localRotation = Quaternion.Slerp(gunRotTrans.localRotation, xSway * ySway * zSway, Time.deltaTime * 5f);
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void gunBobFunc()
    {
        Vector3 gunBobPos;
        Vector3 gunMovePos;

        float swayZPush;
        float frequency;

        //Might be able to move this
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                swayZPush = -0.85f;
                frequency = 17f;

            } else{
                swayZPush = -0.1f;
                frequency = 13.5f;
            }

            gunBobPos = Vector3.zero;

            gunBobPos.x = Mathf.Cos(Time.time * (frequency / 2)) * 0.0325f;
            gunBobPos.y = Mathf.Sin(Time.time * frequency) * 0.045f;
            gunBobPos.z = Mathf.Sin(Time.time * frequency) * 0.025f;

            if(Input.GetAxisRaw("Vertical") != 0) gunBobPos *= Input.GetAxisRaw("Vertical");

            gunMovePos = Vector3.zero;

            gunMovePos.z = gunMoveTrans.localPosition.z + swayZPush;
            gunMovePos.x = gunMoveTrans.localPosition.z + 0.01f * -Input.GetAxisRaw("Horizontal");

            gunMovePos.z = Mathf.Clamp(gunMovePos.z, swayZPush, 0);

        } else {
            gunMovePos = Vector3.zero;

            gunBobPos = Vector3.zero;

            gunBobPos.x = Mathf.Cos(Time.time * 2) * 0.01f;
            gunBobPos.y = Mathf.Sin(Time.time * 4) * -0.025f;
        }

        gunMoveTrans.localPosition = Vector3.Lerp(gunMoveTrans.localPosition, gunBobPos, Time.deltaTime * 3.5f);
        gunMoveTrans.localPosition = Vector3.Lerp(gunMoveTrans.localPosition, gunMovePos, Time.deltaTime);
    }

    //****************************************************************************************************************************************************************************************************************************

    protected float frequencyMultiplierFunc()
    {
        //Control the frequency of the gunbob aka how fast it should bob depending on the players movestate
        var moveState = FindObjectOfType<playerMovement>();

        switch (moveState.state)
        {
            case playerMovement.movementState.sprinting:
                return 2.2f;
            case playerMovement.movementState.crouching:
                return 0.5f;
            default:
                return 1f;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void shootContFunc()
    {
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            cameraMove.mouseXRestrict = 1;
            cameraMove.mouseYRestrict = 1;
        }

        shootFunc();
    }


    public abstract void shootFunc();

    //****************************************************************************************************************************************************************************************************************************

    protected void otherGunFunc()
    {
        reloadFunc();
        altFireFunc();
    }

    protected void reloadFunc()
    {
        if (Input.GetKeyDown(KeyCode.R) && canReload)
            if (totalAmmo != 0 && gunMag != settings.ammoMag)
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName(PICKUPSTATE))
                    coReload = StartCoroutine(IwaitFFS());

        if(anim.GetCurrentAnimatorStateInfo(0).IsName(RELOADSTATE))
            if (Input.GetKeyDown(KeyCode.Mouse0))
                if(gunMag != 0)
                {
                    StopCoroutine(coReload);
                    anim.Play(EQUIPPEDSTATE);
                    canReload = true;
                }
    }

    protected void altFireFunc()
    {
        if (Input.GetKeyDown(KeyCode.V) && settings.canAltFire && anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE))
        {
            if (!altFire) altFire = true;
            else altFire = false;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    protected IEnumerator IwaitFFS()
    {
        canReload = false;

        anim.Play(RELOADSTATE);

        yield return new WaitForSeconds(anim.runtimeAnimatorController.animationClips[1].length);

        int reloadAmount = settings.ammoMag - gunMag;

        if (reloadAmount > totalAmmo)
        {
            gunMag += totalAmmo;
            totalAmmo = 0;

        } else {
            gunMag += reloadAmount;
            totalAmmo -= reloadAmount;
        }

        canReload = true;
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void resetRecoilFunc()
    {
        //Lerp from where our camera is rotoated to zero constantly
        camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, Time.deltaTime * settings.camRecoilReturn);
        //Slerp the cam currentRot to target rot in order to add a bit of delay. However this can also function with the TargetRot, this one is a bit more for the aethestics
        camCurrentRot = Quaternion.Slerp(camCurrentRot, Quaternion.Euler(camTargetRot), Time.fixedDeltaTime * settings.snappiness);
        //make our local rotation the currentRot
        cam.transform.localRotation = camCurrentRot;

        //Move our local position to zero but do not mess with the x and y values in order to avoid clashes in code
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * settings.posRecoilReturnSpeed);
        //Make our rotation always slerp to 0
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * settings.rotRecoilReturnSpeed);
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void recoilFunc()
    {
        float recoilMultiplier;

        /*if (transform.parent == aimArea) recoilMultiplier = settings.aimMultiplier;
        else recoilMultiplier = 1;*/

        recoilMultiplier = 1;

        //Make the cam target rot recoil up a certain amount and add some random recoil in the ranges of the -Y and the poitive Y, same for the Z axis
        camTargetRot += new Vector3(-settings.camRecoilX * recoilMultiplier, UnityEngine.Random.Range(-settings.camRecoilY, settings.camRecoilY) * recoilMultiplier, UnityEngine.Random.Range(-settings.camRecoilZ, settings.camRecoilZ) * recoilMultiplier);

        //Rotate our gun as far back as we want it, depending on the settings
        transform.localRotation = Quaternion.Euler(-settings.recoilTargetRot * recoilMultiplier, 0, 0);

        //Get the local position of our gun
        Vector3 posRecoil = transform.localPosition;
        //Subtract the position by the target push backwars
        posRecoil -= Vector3.forward * settings.recoilTargetPushZ;
        posRecoil += Vector3.up * settings.recoilTargetPushY;

        //Clamp it to a specified amount in order to avoid a super far back recoil
        posRecoil.z = Mathf.Clamp(posRecoil.z, -settings.maxPosRecoilZ - gunMoveTrans.localPosition.z, 0);
        posRecoil.y = Mathf.Clamp(posRecoil.y, 0, settings.maxPosRecoilY + gunMoveTrans.localPosition.y);
        //Set the position to be the recoil. Can this be more efficient? Yes. Did this bull shit take me so long even though it's the easiest thing ever and is 100% not what humbled my ego in this project? Yes. So will I fix it? Idk.
        transform.localPosition = posRecoil;
    }

    //****************************************************************************************************************************************************************************************************************************
}
