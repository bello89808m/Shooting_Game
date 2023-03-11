using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class interactController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float distance;

    [Header("Cursor")]
    [SerializeField] private GameObject InteractingHoldGo;
    [SerializeField] private Image interactionProgress;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Important Place Transforms")]
    [SerializeField] private Transform enviroment;

    [Header("Pick Up System")]
    [SerializeField] private GameObject pickUpObj = null;
    [SerializeField] private GameObject[] objInv = new GameObject[3];
    private bool setObjtoNull = true;
    private int holdingNum = 0;
    private int lastHoldingNum;
    

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;

        if (setObjtoNull) pickUpObj = null;

        if (Physics.Raycast(ray, out hit, distance, LayerMask.GetMask("Default")))
        {
            InteractingHoldGo.SetActive(true);

            if (hit.collider.TryGetComponent<Interactable>(out Interactable interacting))
            {
                hitSomething = true;
                description.text = interacting.getDescription();

                if (!interacting.canInteract) return;

                //If we actually interact with it
                HandleInteraction(interacting);

            } else if (hit.collider.TryGetComponent<pickUp>(out pickUp selectedObj)) {
                //Activate UI
                hitSomething = true;

                //If we decide to interact with it
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (pickUpObj == null)
                    {
                        objInv[holdingNum] = hit.transform.gameObject;

                    } else if (pickUpObj != null) {
                        drop(hit.transform);

                        objInv[holdingNum] = hit.transform.gameObject;
                    }

                    pickUp(hit.transform.gameObject, selectedObj.getTransformArea());

                    setObjtoNull = false;

                } 
            } else if (hit.collider.TryGetComponent<place>(out place placeObj)) {

                if (pickUpObj != null) {
                    hitSomething = true;

                    if(Input.GetKeyDown(KeyCode.Mouse0)) drop(placeObj.getPlaceArea());
                }
            }
        }

        if (!hitSomething) {InteractingHoldGo.SetActive(false); description.text = ""; }

        pickUpObj = objInv[holdingNum] != null ? objInv[holdingNum] : null;

        inputs();

        foreach (GameObject holdObjCheck in objInv)
        {
            if (holdObjCheck == objInv[holdingNum] && holdObjCheck != null)
            {
                holdObjCheck.SetActive(true);

            } else if (holdObjCheck != objInv[holdingNum] && holdObjCheck != null) {
                holdObjCheck.SetActive(false);

            }
        }

        if (objInv.Length <= 0) setObjtoNull = true;
    }

    //**************************************************************************************************************

    void HandleInteraction(Interactable interactable)
    {
        switch (interactable.interactiontype)
        {
            //If we want the type to be click
            case Interactable.InteractionType.Click:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.interact();
                }
                break;

            //If we want the type to be hold
            case Interactable.InteractionType.Hold:
                if (Input.GetKey(KeyCode.E))
                {
                    interactable.holdingTime();
                    interactionProgress.fillAmount = interactable.getHoldTime();

                    if (interactable.getHoldTime() > 1.0f)
                    {
                        interactable.interact();
                        interactable.resetTime();
                    }

                } else {
                    interactable.resetTime();
                    interactionProgress.fillAmount = 1;
                }

                break;

            //When the result is null
            default:
                throw new System.Exception("Unsupported Interactable");
        }
    }

    //**************************************************************************************************************

    void pickUp(GameObject pickUp, Transform holdArea)
    {
        pickUp.transform.SetParent(holdArea);
        pickUp.transform.SetSiblingIndex(holdingNum);

        pickUp.transform.localPosition = Vector3.zero;
        pickUp.transform.localEulerAngles = Vector3.zero;
        pickUp.transform.localScale = Vector3.one;

        pickUp.layer = LayerMask.NameToLayer("Holding");
    }

    //**************************************************************************************************************

    void drop(Transform placeArea)
    {
        objInv[holdingNum] = null;

        pickUpObj.transform.SetParent(placeArea);

        pickUpObj.transform.localPosition = Vector3.zero;
        pickUpObj.transform.localEulerAngles = Vector3.zero;
        pickUpObj.transform.localScale = Vector3.one;

        pickUpObj.layer = LayerMask.NameToLayer("Default");

        pickUpObj.transform.SetParent(enviroment);
    }

    //**************************************************************************************************************

    void inputs()
    {
        //Scrolling up
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            lastHoldingNum = holdingNum;
            holdingNum++;

            //Make sure we can't scroll past 3
            if (holdingNum > 2) holdingNum = 0;
        }
        //Scrolling down
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            lastHoldingNum = holdingNum;
            holdingNum--;

            //Make sure we can't scroll past 0
            if (holdingNum < 0) holdingNum = 2;
        }
        //Self explanatory if the person reading this isn't brain dead

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lastHoldingNum = holdingNum;
            holdingNum = 0;

        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            lastHoldingNum = holdingNum;
            holdingNum = 1;

        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            lastHoldingNum = holdingNum;
            holdingNum = 2;

        } else if (Input.GetKeyDown(KeyCode.Q)) {
            int holdingNumHolder = holdingNum;
            holdingNum = lastHoldingNum;
            lastHoldingNum = holdingNumHolder;
        }
    }
}
