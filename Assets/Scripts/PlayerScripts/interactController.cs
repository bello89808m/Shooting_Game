using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace player
{
    public class interactController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera cam;
        [SerializeField] private float distance;
        RaycastHit hit;

        [Header("Cursor")]
        [SerializeField] private GameObject isLookingAtCursor;
        [SerializeField] private Image interactionProgress;
        [SerializeField] private GameObject cursor;
        [SerializeField] private TextMeshProUGUI description;

        [Header("Pick Up System")]
        private GameObject[] objInv = new GameObject[3];
        public GameObject pickUpObj { get; private set; }
        private int holdingNum = 0;
        private int lastHoldingNum;
        private Transform originalTrans;
        public bool hitSomething { get; private set; }
        private Animator objAnim;

        [Header("Interact System")]
        private float lastInteractTime = 0;
        private float holdTime = 0;

        void Update()
        {
            //Launch a ray from the center of the camera
            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
            hitSomething = false;

            //Launch a raycast from the center of the camera and only hit things with the Default layer mask
            if (Physics.Raycast(ray, out hit, distance, LayerMask.GetMask("Default")))
            {
                //if we hit an object that's interactable
                if (hit.collider.TryGetComponent<Interactable>(out Interactable interacting))
                {
                    hitSomething = true;
                    isLookingAtCursor.SetActive(true);
                    //Get what we're doing with the interactable
                    description.text = interacting.getDescription();

                    //If we actually interact with it/press E
                    HandleInteraction(interacting);

                    //If we look at something that can be picked up
                } else if (hit.collider.TryGetComponent<IPick>(out IPick selectedObj)) {

                    hitSomething = true;
                    //Set the cursor active 
                    isLookingAtCursor.SetActive(true);
                    pickUpSystem(selectedObj);

                    //If we look at something where we can place our object
                } else if (hit.collider.TryGetComponent<IPlace>(out IPlace placeObj)) {

                    hitSomething = true;
                    //Set the cursor active
                    isLookingAtCursor.SetActive(true);
                    //Check we aren't holding are
                    if (pickUpObj != null)
                    {
                        //Get what we're doing with the interactable
                        description.text = placeObj.getDescription(pickUpObj);

                        //if we click it run the place script
                        if (Input.GetKeyDown(KeyCode.Mouse0)) drop(placeObj.getPlaceArea());
                    }
                }
            }

            //if we are not hitting something, set these things to null except not really null but just pretend ok
            if (!hitSomething) { isLookingAtCursor.SetActive(false); description.text = ""; }

            //Set our pick up obj to whatever we're holding
            pickUpObj = objInv[holdingNum] != null ? objInv[holdingNum] : null;
            objAnim = pickUpObj != null ? pickUpObj.GetComponentInChildren<Animator>() : null;

            //Check for our inputs
            inputs();
        }

        //**************************************************************************************************************

        public void HandleInteraction(Interactable interactable)
        {
            switch (interactable.getType())
            {
                //If we want the type to be click
                case Interactable.InteractionType.Click:

                    if (!interactable.getDownTime())
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            //run the interact function
                            interactable.interact();
                        }
                    } else {
                        isLookingAtCursor.SetActive(false);
                        description.text = "";
                    }

                    break;

                //If we want the type to be hold
                case Interactable.InteractionType.Hold:

                    //Downtime. Will add to the other version as well, but may need change the text part.
                    if(!interactable.getDownTime())
                    {
                        if (Input.GetKey(KeyCode.E))
                        {
                            //start a timer where we add to a float using deltaTime
                            holdTime += Time.deltaTime;
                            //set the amount filled to correspond to how long you've been holding it
                            interactionProgress.fillAmount = holdTime;

                            //if we go the full way
                            if (holdTime > 1.0f)
                            {
                                //run the interact script then set the time to 0
                                interactable.interact();
                                holdTime = 0;
                                lastInteractTime = Time.time;
                            }

                        } else {
                            //if we let go, reset the time and set the progress amount to 1, so we don't break the cursor
                            holdTime = 0;
                            interactionProgress.fillAmount = 1;
                        }

                    } else {
                        isLookingAtCursor.SetActive(false);
                        description.text = "";
                    }

                    break;

                //When the result is null
                default:
                    throw new System.Exception("Unsupported Interactable");
            }
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

            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                lastHoldingNum = holdingNum;
                holdingNum = 1;

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                lastHoldingNum = holdingNum;
                holdingNum = 2;

                //Make it so we can press Q and switch to the thing we were holding last
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                int holdingNumHolder = holdingNum;
                holdingNum = lastHoldingNum;
                lastHoldingNum = holdingNumHolder;
            }

            inventorySorter();

            if(objAnim != null) objAnim.SetTrigger("pickUp");
        }

        //**************************************************************************************************************

        void inventorySorter()
        {

            foreach (GameObject holdObjCheck in objInv)
            {
                if (holdObjCheck == objInv[holdingNum] && holdObjCheck != null)
                {
                    holdObjCheck.SetActive(true);

                }
                else if (holdObjCheck != objInv[holdingNum] && holdObjCheck != null)
                {
                    holdObjCheck.SetActive(false);
                }
            }
        }

        //**************************************************************************************************************

        void pickUpSystem(IPick selectedObj)
        {
            //Get what we're doing with the interactable
            description.text = selectedObj.getDesc();

            //If we decide to interact with it
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //Check if the thing we're holding isn't air
                if (pickUpObj == null)
                {
                    //Add to our inventory 
                    objInv[holdingNum] = hit.transform.gameObject;

                } else if (pickUpObj != null) {
                    drop(hit.transform);

                    objInv[holdingNum] = hit.transform.gameObject;
                }

                //Run the pick up function to set the object into our transform
                pickUp(hit.transform.gameObject, selectedObj.getTransformArea());
            }
        }

        //**************************************************************************************************************

        void pickUp(GameObject pickUp, Transform holdArea)
        {
            //Set out object to be parented to where we want it to go after holding it
            pickUp.transform.SetParent(holdArea);
            //idk this is a quality of life thing for me I just like it
            pickUp.transform.SetSiblingIndex(holdingNum);

            //Make sure that the rotations, positions, and scale match to what we want when picking it up
            StartCoroutine(moveObj(pickUp));
            pickUp.transform.localScale = Vector3.one;

            //Set the layer to holding for the camera
            pickUp.layer = LayerMask.NameToLayer("Holding");
        }

        IEnumerator moveObj(GameObject pickUp)
        {
            while(pickUp.transform.localPosition != Vector3.zero)
            {
                pickUp.transform.localPosition = Vector3.MoveTowards(pickUp.transform.localPosition, Vector3.zero, Time.deltaTime * 2f);
                pickUp.transform.localRotation = Quaternion.Slerp(pickUp.transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * 2f);
                yield return null;
            }
        }

        //**************************************************************************************************************

        void drop(Transform placeArea)
        {
            objAnim.SetTrigger("resetAnim");
            objAnim.ResetTrigger("pickUp");

            //Set the parent to where we want to place it
            pickUpObj.transform.SetParent(placeArea);

            //Make sure that the rotations, positions, and scale match to what we want when placing it
            pickUpObj.transform.localPosition = Vector3.zero;
            pickUpObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            pickUpObj.transform.localScale = Vector3.one;

            //Make the layer the default, enviroment layer
            pickUpObj.layer = LayerMask.NameToLayer("Default");

            pickUpObj.transform.SetParent(null);

            //remove the object from our inventory
            objInv[holdingNum] = null;
        }
    }
}
