using System.Collections;
using System.Collections.Generic;
using player;
using TMPro;
using UnityEngine;

public abstract class GunClass : MonoBehaviour, IPick, IFunction
{
    [Header("Gun Transforms")]
    [SerializeField] protected Transform holdArea;
    [SerializeField] protected Transform aimArea;
    [SerializeField] protected Transform shootArea;

    [Header("Gun Settings")]
    [SerializeField] protected gunSettingsScriptable settings;

    [Header("Player Cam")]
    protected Camera cam;
    protected RaycastHit hit;

    [Header("Ammo Count")]
    [SerializeField] protected TextMeshProUGUI ammoCount;

    [Header("Crosshair")]
    [SerializeField] protected GameObject cursor;
    [SerializeField] protected RectTransform crossHair;
    public GameObject crossGetter { get; private set; }
    protected static float crossSize;

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
    
    //Gun Bob
    protected Vector3 gunBobPos = Vector3.zero;

    //Ammo
    public int gunMag;
    public int totalAmmo { get; private set; }

    //Reload
    public static bool showAmmo;
    public static string ammo;
    protected bool canReload = true;
    protected Coroutine coReload;

    //Aiming
    private bool isAiming;
    protected mouseLook cameraMove;
    protected interactController interactCont; 

    //Alt Fire
    protected bool altFire = false;

    //IPick settings
    public string getDescFunc() => "Pick up " + settings.gunName;
    public Transform getTransformAreaFunc() => holdArea;

    //IFunction settings
    public void doThisFunc()
    {
        shootContFunc();

        otherGunFunc();
        gunMoveControllerFunc();
        resetValFunc();
        dynamicCrosshairMoveFunc();
    }

    public void resetValuesFunc()
    {
        //Make us stop aiming
        isAiming = false;
        transform.SetParent(holdArea);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        canReload = true;

        cameraMove.mouseXRestrict = 1;
        cameraMove.mouseYRestrict = 1;

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
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void Update()
    {
        ammoCount.SetText(ammo);
        ammoCount.enabled = showAmmo;
        //Check if the gun is actually held first

        if (transform.parent == holdArea || transform.parent == aimArea)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(IDLESTATE)) anim.Play(PICKUPSTATE);
            else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) anim.Play(EQUIPPEDSTATE);

        } else {
            anim.Play(IDLESTATE);
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
        if (Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical") != 0)
        {
            crossSize = Mathf.Lerp(crossSize, settings.movingCross, Time.deltaTime * settings.crossSpeed);

        } else {
            crossSize = Mathf.Lerp(crossSize, settings.sittingCross, Time.deltaTime * settings.crossSpeed);
        }
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
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            //Get the axis of us turning the mouse and multiply it by how powerful we want the sway to be
            float x = Input.GetAxisRaw("Mouse X") * 200;
            float y = Input.GetAxisRaw("Mouse Y") * 225;

            float mouseSwayPushX;
            float mouseSwayPushY;
            Vector3 mouseSwayPush;

            if (Input.GetAxisRaw("Mouse X") > 0) mouseSwayPushX = transform.localPosition.x + settings.swayPosDelay;
            else if (Input.GetAxisRaw("Mouse X") < 0) mouseSwayPushX = transform.localPosition.x - settings.swayPosDelay;
            else mouseSwayPushX = 0;

            if (Input.GetAxisRaw("Mouse Y") > 0) mouseSwayPushY = transform.localPosition.y + settings.swayPosDelay;
            else if (Input.GetAxisRaw("Mouse Y") < 0) mouseSwayPushY = transform.localPosition.y - settings.swayPosDelay;
            else mouseSwayPushY = 0;

            mouseSwayPush = new Vector2(mouseSwayPushX, mouseSwayPushY);

            //turn the gun on the direction our mouse is going by getting the sway and rotating it around the corresponding axis
            Quaternion xSway = Quaternion.AngleAxis(-x, Vector3.up);
            Quaternion ySway = Quaternion.AngleAxis(y, Vector3.right);

            //change the actual rotation of the gun itself
            transform.localRotation = Quaternion.Slerp(transform.localRotation, xSway * ySway, Time.deltaTime * settings.rotSway);
            transform.localPosition = Vector3.Lerp(transform.localPosition, mouseSwayPush, Time.deltaTime);
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void gunBobFunc()
    {
        //Might be able to move this
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float swayMultiplier = frequencyMultiplierFunc();
            float swayZPush;
            float swayXPush;

            if (Input.GetAxisRaw("Vertical") == 1) swayZPush = settings.forwardPush;
            else if (Input.GetAxisRaw("Vertical") == -1) swayZPush = -settings.backwardPush;
            else swayZPush = 0;

            if (Input.GetAxisRaw("Horizontal") == 1) swayXPush = settings.horizontalPush;
            else if (Input.GetAxisRaw("Horizontal") == -1) swayXPush = -settings.horizontalPush;
            else swayXPush = 0;

            gunBobPos.x = Mathf.Cos(Time.time * (settings.frequency / 2 * swayMultiplier)) * settings.amplitude / 5.5f - swayXPush;
            gunBobPos.y = Mathf.Sin(Time.time * (settings.frequency * swayMultiplier)) * settings.amplitude / 2.5f;
            gunBobPos.z = transform.localPosition.z - swayZPush;

            transform.localPosition = Vector3.Lerp(transform.localPosition, gunBobPos, Time.deltaTime);
        }
    }

    //***************************************************************************************************************************************************************************************************************************

    //****************************************************************************************************************************************************************************************************************************

    protected float frequencyMultiplierFunc()
    {
        //Control the frequency of the gunbob aka how fast it should bob depending on the players movestate
        var moveState = FindObjectOfType<playerMovement>();

        switch (moveState.state)
        {
            case playerMovement.movementState.sprinting:
                return 2.3f;
            case playerMovement.movementState.crouching:
                return 1f;
            default:
                return 1.4f;
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

    protected void otherGunFunc()
    {
        if (Input.GetKeyDown(KeyCode.R) && canReload)
            if (totalAmmo != 0 && gunMag != settings.ammoMag)
                if (anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE))
                    coReload = StartCoroutine(IwaitFFS());

        if (Input.GetKey(KeyCode.Mouse1) && settings.canAim && canReload) aimingFunc();
        else if (isAiming && !Input.GetKey(KeyCode.Mouse1)) stopAimingFunc();

        if(Input.GetKeyDown(KeyCode.V) && settings.canAltFire && anim.GetCurrentAnimatorStateInfo(0).IsName(EQUIPPEDSTATE))
        {
            if (!altFire) altFire = true;
            else altFire = false;
        }
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void aimingFunc()
    {
        isAiming = true;

        transform.SetParent(aimArea);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);

        cursor.SetActive(false);
    }

    protected void stopAimingFunc()
    {
        cursor.SetActive(true);

        transform.SetParent(holdArea);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);

        if (transform.localPosition == Vector3.zero) isAiming = false;
    }

    //****************************************************************************************************************************************************************************************************************************

    protected IEnumerator IwaitFFS()
    {
        if (isAiming)
        {
            cursor.SetActive(true);

            transform.SetParent(holdArea);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);

            StartCoroutine(IstopAim());
        }

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
        anim.Play(EQUIPPEDSTATE);
    }

    private IEnumerator IstopAim()
    {
        while(transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 5f);

            yield return null;
        }

        isAiming = false;
    }

    //****************************************************************************************************************************************************************************************************************************

    protected void resetValFunc()
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

        if (transform.parent == aimArea) recoilMultiplier = settings.aimMultiplier;
        else recoilMultiplier = 1;

        //Make the cam target rot recoil up a certain amount and add some random recoil in the ranges of the -Y and the poitive Y, same for the Z axis
        camTargetRot += new Vector3(-settings.camRecoilX * recoilMultiplier, UnityEngine.Random.Range(-settings.camRecoilY, settings.camRecoilY) * recoilMultiplier, UnityEngine.Random.Range(-settings.camRecoilZ, settings.camRecoilZ) * recoilMultiplier);

        //Rotate our gun as far back as we want it, depending on the settings
        transform.localRotation = Quaternion.Euler(-settings.recoilTargetRot * recoilMultiplier, 0, 0);

        //Get the local position of our gun
        Vector3 posRecoil = transform.localPosition;
        //Subtract the position by the target push backwars
        posRecoil -= Vector3.forward * settings.recoilTargetPush;
        //Clamp it to a specified amount in order to avoid a super far back recoil
        posRecoil.z = Mathf.Clamp(posRecoil.z, -settings.maxPosRecoil + transform.localPosition.z, 0);
        //Set the position to be the recoil. Can this be more efficient? Yes. Did this bull shit take me so long even though it's the easiest thing ever and is 100% not what humbled my ego in this project? Yes. So will I fix it? Idk.
        transform.localPosition = posRecoil;
    }

    //****************************************************************************************************************************************************************************************************************************

    public abstract void shootFunc();
}
