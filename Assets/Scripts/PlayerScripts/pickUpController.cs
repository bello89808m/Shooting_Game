using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class pickUpController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance;

    [SerializeField] private TextMeshProUGUI HoldText; //Cause unity won't let me reuse another fucking text box

    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform enviroment;

    [SerializeField]private GameObject pickUpObj = null;
    [SerializeField] private Rigidbody pickUpBody = null;
    [SerializeField]private GameObject[] objInv = new GameObject[3];
    [SerializeField] private bool setObjtoNull = true;
    [SerializeField] private int holdingNum = 0;
    private int lastHoldingNum;

    void Update()
    {
        //If we're not holding anything, make sure these are null
        if (setObjtoNull) { pickUpObj = null; pickUpBody = null; }

        //Launch a raycast from the center of our camera
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, distance, LayerMask.GetMask("Default")))
        {
            //If it hits with something that has the IHold interface
            if (hit.collider.TryGetComponent<pickUp>(out pickUp selectedObj))
            {
                //Activate UI
                hitSomething = true;

                HoldText.text = selectedObj.getDescription();

                //If we decide to interact with it
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (pickUpObj == null)
                    {
                        objInv[holdingNum] = hit.transform.gameObject;

                    } else if(pickUpObj != null) {
                        drop(enviroment, hit.transform);

                        objInv[holdingNum] = hit.transform.gameObject;
                    }

                    pickUp(hit.transform.gameObject, selectedObj.getObjBody(), selectedObj.getTransformArea());

                    setObjtoNull = false;
                }
            }
        }

        //If we're not hitting something, make sure theres no text on screen
        if (!hitSomething)
        {
            HoldText.text = "";
        }

        pickUpObj = objInv[holdingNum] != null ? objInv[holdingNum] : null;
        pickUpBody = objInv[holdingNum] != null ? objInv[holdingNum].GetComponent<Rigidbody>() : null;

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

    public void pickUp(GameObject pickUp, Rigidbody objBody, Transform placeArea)
    {
        pickUp.transform.SetParent(placeArea);
        pickUp.transform.SetSiblingIndex(holdingNum);

        pickUp.transform.localPosition = Vector3.zero;
        pickUp.transform.localEulerAngles = Vector3.zero;
        pickUp.transform.localScale = Vector3.one;

        pickUp.layer = LayerMask.NameToLayer("Holding");

        objBody.isKinematic = true;
    }

    //WIP
    public void drop(Transform placeArea, Transform placeAreaPosition)
    {
        pickUpObj.transform.SetParent(placeArea);

        pickUpObj.transform.localPosition = placeAreaPosition.position;
        pickUpObj.transform.localEulerAngles = placeAreaPosition.eulerAngles;
        pickUpObj.transform.localScale = Vector3.one;

        pickUpObj.layer = LayerMask.NameToLayer("Default");

        pickUpBody.isKinematic = true;
    }


    public void inputs()
    {
        //Scrolling up
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            lastHolding(holdingNum);
            holdingNum++;

            //Make sure we can't scroll past 3
            if (holdingNum > 2)
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

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lastHolding(holdingNum);
            holdingNum = 0;

        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            lastHolding(holdingNum);
            holdingNum = 1;

        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            lastHolding(holdingNum);
            holdingNum = 2;

        } else if (Input.GetKeyDown(KeyCode.Q)) {
            int holdingNumHolder = holdingNum;
            holdingNum = lastHoldingNum;
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
            case 2:
                lastHoldingNum = 2;
                break;
            case 1:
                lastHoldingNum = 1;
                break;
        }
    }
}
