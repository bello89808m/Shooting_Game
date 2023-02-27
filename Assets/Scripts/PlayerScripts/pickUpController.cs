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

    private GameObject pickUpObj = null;
    private Rigidbody objBody = null;
    private bool canPickUp = true;
    private int holdingNum = -1;
    private int? lastHoldingNum;

    void Update()
    {
        //If we're not holding anything, make sure these are null
        if (canPickUp){pickUpObj = null; objBody = null;}

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
                if (Input.GetKeyDown(KeyCode.E) && holdArea.childCount != 3)
                {
                    //Put the object in our holding area and set pickUpObj and objBody to the object we interacted with
                    pickUp(hit);

                    //Make sure the object is in the right place of its hierarchy (SetAsLastSibling didn't work fuck me)
                    pickUpObj.transform.SetSiblingIndex(holdArea.childCount);

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
                    } else {
                        holdingNum += 1;
                        lastHoldingNum = holdArea.childCount > 1 ? holdingNum - 1 : null;
                    }
                //If we interact with an IHold but we have full inventory
                } else if (Input.GetKeyDown(KeyCode.E) && holdArea.childCount == 3) {

                    //drop the current object we're holding
                    drop();

                    //Make sure we make the new object active after dropping it
                    pickUpObj.SetActive(true);

                    //Add the new object to our player
                    pickUp(hit);
                    pickUpObj.transform.SetSiblingIndex(holdingNum);

                    selectedObj.onInteract();

                    canPickUp = false;
                }
            }

        //If we're not hitting something, make sure theres no text on screen
        }if (!hitSomething) HoldText.text = "";

        //Check if we actually picked something up or not
        if(holdingNum != -1)
        {
            //Check for any player inputs
            inputs();

            //This number will be what we use in order to see what we have equipped or not
            int itemSelected = 0;
            //For every object we picked up, run it through this loop. If the itemSelected num is equal to our holding num,
            //make it so that the object with the corresponding number is set active. If not, dont show it. Then run through again but
            //with itemSelected increased to check if we changed our holding number or not.
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
    public void pickUp(RaycastHit hit)
    {
        objBody = hit.transform.GetComponent<Rigidbody>();
        pickUpObj = hit.collider.gameObject;

        pickUpObj.transform.SetParent(holdArea);
    }

    //WIP
    void drop()
    {
        pickUpObj.transform.SetParent(enviroment);

        pickUpObj.transform.localScale = Vector3.one;

        pickUpObj.layer = 0;

        objBody.velocity = playerVelocity.velocity;

        objBody.isKinematic = false;
         objBody.AddForce(cam.transform.forward * 4.5f, ForceMode.Impulse);
          objBody.AddForce(cam.transform.right * -1.25f, ForceMode.Impulse);
           objBody.AddForce(cam.transform.up, ForceMode.Impulse);

         float randomRange = Random.Range(-1, 1);
          objBody.AddTorque(new Vector3(randomRange, randomRange, randomRange) * 30);

        canPickUp = true;
    }


    void inputs()
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
        //Self explanatory if the person reading this isn't brain dead with a lil bit of twitter rot
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
