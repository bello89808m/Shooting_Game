using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pickUpController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance;

    [SerializeField] private TextMeshProUGUI HoldText; //Cause unity won't let me reuse another fucking text box

    [SerializeField] private CharacterController playerVelocity;
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform enviroment;

    [SerializeField]private GameObject pickUpObj = null;
    private Rigidbody objBody = null;
    private bool canPickUp = true;
    [SerializeField] private int holdingNum = -1;
    private int? lastHoldingNum;

    void Update()
    {
        //If we're not holding anything, make sure these are null
        if (canPickUp) { pickUpObj = null; objBody = null; }

        //Launch a raycast from the center of our camera
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, distance))
        {
            //If it hits with something that has the IHold interface
            if (hit.collider.TryGetComponent<IHold>(out IHold selectedObj))
            {
                //Activate UI
                hitSomething = true;

                HoldText.text = selectedObj.getDescription();

                //If we decide to interact with it
                if (Input.GetKeyDown(KeyCode.Mouse0) && holdArea.childCount != 3)
                {
                    pickUp(hit, selectedObj, holdArea.childCount);

                    //Run the interact function on the object we picked up, eg making it scale, make the rotation right, etc
                    selectedObj.onInteract();

                    //We can no longer pick up
                    canPickUp = false;

                    //Check if we have the first object out or not and make sure we have only two objects picked up.
                    //If it's the first one out and we pick up a new object, make sure we have the newly picked up object equipped.


                    if (holdingNum == 0 && holdArea.childCount == 3)
                    {
                        holdingNum = 2;
                        lastHoldingNum = 0;
                    }
                    else
                    {
                        holdingNum += 1;
                        lastHoldingNum = holdArea.childCount > 1 ? holdingNum - 1 : null;
                    }
                }
            } else if (hit.collider.TryGetComponent<placeController>(out placeController placeArea)) {
                hitSomething = true;

                //If we decide to interact with it
                if (Input.GetKeyDown(KeyCode.Mouse0) && pickUpObj != null)
                {
                    Debug.Log("fuck");
                    drop(placeArea.transform);
                }
            }
        }

        //If we're not hitting something, make sure theres no text on screen
        if (!hitSomething)
        {
            HoldText.text = "";
        }

        //Check if we actually picked something up or not
        if (holdingNum != -1)
        {
            inputs();

            //This number will be what we use in order to see what we have equipped or not
            int itemSelected = 0;
            foreach (Transform holdingObjType in holdArea)
            {
                if (itemSelected == holdingNum)
                {
                    holdingObjType.gameObject.SetActive(true);
                    pickUpObj = holdingObjType.gameObject;
                    objBody = holdingObjType.gameObject.GetComponent<Rigidbody>();

                }
                else
                {
                    holdingObjType.gameObject.SetActive(false);
                }
                itemSelected++;
            }
        }
    }

    //Explained above
    public void pickUp(RaycastHit hit, IHold hold, int objIndex)
    {
        objBody = hit.transform.GetComponent<Rigidbody>();
        pickUpObj = hit.collider.gameObject;

        pickUpObj.transform.SetParent(holdArea);

        pickUpObj.transform.SetSiblingIndex(objIndex);

        hold.onInteract();

        canPickUp = false;
    }

    //WIP
    public void drop(Transform placeArea)
    {
        Debug.Log("FUCCUCUCUCUCUCUCUK");
        foreach (Transform place in placeArea)
        {
            Debug.Log(place.childCount);
            if (place.childCount == 0) {
                pickUpObj.transform.SetParent(place);

                pickUpObj.transform.localScale = Vector3.one;
                pickUpObj.transform.localScale = Vector3.zero;

                break;
            }
        }

        pickUpObj.layer = 0;

        canPickUp = true;

        holdingNum--;
    }


    public void inputs()
    {
        //Scrolling up
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            lastHolding(holdingNum);
            holdingNum++;

            //Make sure we can't scroll past 3
            if (holdingNum > holdArea.childCount - 1)
            {
                holdingNum = 0;
            }
        }
        //Scrolling down
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            lastHolding(holdingNum);
            holdingNum--;

            //Make sure we can't scroll past 0
            if (holdingNum < 0)
            {
                holdingNum = holdArea.childCount - 1;
            }
        }
        //Self explanatory if the person reading this isn't brain dead

        else if (Input.GetKeyDown(KeyCode.Alpha1) && holdArea.childCount > 0)
        {
            lastHolding(holdingNum);
            holdingNum = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && holdArea.childCount > 1)
        {
            lastHolding(holdingNum);
            holdingNum = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && holdArea.childCount > 2)
        {
            lastHolding(holdingNum);
            holdingNum = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && lastHoldingNum != null)
        {
            int holdingNumHolder = holdingNum;
            holdingNum = (int)lastHoldingNum;
            lastHoldingNum = holdingNumHolder;
        }
    }

    //What allows me to do the weapon switching
    public void lastHolding(int num)
    {
        switch (num)
        {
            case 0:
                lastHoldingNum = 0;
                break;
            case 1:
                lastHoldingNum = 1;
                break;
            case 2:
                lastHoldingNum = 2;
                break;
        }
    }
}
