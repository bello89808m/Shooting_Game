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
        [SerializeField] private GameObject cursor;
        [SerializeField] private GameObject isLookingAtCursor;
        [SerializeField] private Image interactionProgress;
        [SerializeField] private TextMeshProUGUI description;
        private GameObject crossHair;

        [Header("Pick Up System")]
        private GameObject[] objInv = new GameObject[3];
        private int holdingNum = 0;
        private int lastHoldingNum;

        public GameObject pickUpObj { get; private set; }
        public bool hitSomething { get; private set; }
        public Animator objAnim { get; private set; }

        [Header("Interact System")]
        private float holdTime = 0;

        void Update()
        {
            //Launch a ray from the center of the camera
            Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
            hitSomething = false;

            //Launch a raycast from the center of the camera and only hit things with the Default layer mask
            if (Physics.Raycast(ray, out hit, distance, ~LayerMask.GetMask("Holding")))
            {
                //if we hit an object that's interactable
                if (hit.collider.TryGetComponent(out Interactable interacting))
                {
                    hitSomething = true;
                    isLookingAtCursor.SetActive(true);
                    //Get what we're doing with the interactable
                    description.text = interacting.getDescriptionFunc();

                    //If we actually interact with it/press E
                    HandleInteractionFunc(interacting);

                    //If we look at something that can be picked up
                } else if (hit.collider.TryGetComponent(out IPick selectedObj)) {

                    hitSomething = true;
                    //Set the cursor active 
                    isLookingAtCursor.SetActive(true);
                    pickUpSystemFunc(selectedObj);

                    //If we look at something where we can place our object
                } else if (hit.collider.TryGetComponent(out IPlace placeObj)) {

                    hitSomething = true;
                    //Set the cursor active
                    isLookingAtCursor.SetActive(true);
                    //Check we aren't holding are
                    dropSystemFunc(placeObj);
                }
            }

            //if we are not hitting something, set these things to null except not really null but just pretend ok
            if (!hitSomething) { isLookingAtCursor.SetActive(false); description.text = ""; }

            //Set our pick up obj to whatever we're holding
            pickUpObj = objInv[holdingNum] != null ? objInv[holdingNum] : null;
            objAnim = pickUpObj != null ? pickUpObj.GetComponentInChildren<Animator>() : null;

            //Check for our inputs
            inputsFunc();

            handleGunFunc();
        }

        //**************************************************************************************************************

        public void HandleInteractionFunc(Interactable interactable)
        {
            switch (interactable.getTypeFunc())
            {
                //If we want the type to be click
                case Interactable.InteractionType.Click:

                    if (!interactable.getDownTimeFunc())
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            //run the interact function
                            interactable.interactFunc();
                        }
                    } else {
                        isLookingAtCursor.SetActive(false);
                        description.text = "";
                    }

                    break;

                //If we want the type to be hold
                case Interactable.InteractionType.Hold:

                    //Downtime. Will add to the other version as well, but may need change the text part.
                    if(!interactable.getDownTimeFunc())
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
                                interactable.interactFunc();
                                holdTime = 0;
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

        void inputsFunc()
        {
            void changeItemsFunc(int lastNum, int num)
            {
                if (num == holdingNum) return;

                lastHoldingNum = lastNum;
                holdingNum = num;

                inventorySorterFunc();

                if (pickUpObj != null)
                    if (pickUpObj.TryGetComponent(out IFunction resetObj))
                        resetObj.resetValuesFunc();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                changeItemsFunc(holdingNum, 0);

            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                changeItemsFunc(holdingNum, 1);

            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                changeItemsFunc(holdingNum, 2);

                //Make it so we can press Q and switch to the thing we were holding last
            } else if (Input.GetKeyDown(KeyCode.Q)) {
                int holdingNumHolder = holdingNum;

                changeItemsFunc(holdingNumHolder, lastHoldingNum);
            }
        }

        //**************************************************************************************************************

        void inventorySorterFunc()
        {
            foreach (GameObject holdObjCheck in objInv)
            {
                if (holdObjCheck == null) continue;

                if (holdObjCheck == objInv[holdingNum])
                {
                    holdObjCheck.SetActive(true);

                } else if (holdObjCheck != objInv[holdingNum]) {
                    holdObjCheck.SetActive(false);
                }
            }
        }

        //**************************************************************************************************************

        void pickUpSystemFunc(IPick selectedObj)
        {
            //Get what we're doing with the interactable
            description.text = selectedObj.getDescFunc();

            //If we decide to interact with it
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //Check if the thing we're holding isn't air
                if (pickUpObj == null)
                {
                    //Add to our inventory 
                    objInv[holdingNum] = hit.transform.gameObject;

                } else if (pickUpObj != null) {

                    if (!objAnim.GetCurrentAnimatorStateInfo(0).IsName("equipped")) return;
                    dropFunc(hit.transform);
                }

                //Run the pick up function to set the object into our transform
                pickUpFunc(hit.transform.gameObject, selectedObj.getTransformAreaFunc());
            }
        }

        //**************************************************************************************************************

        void pickUpFunc(GameObject pickUp, Transform holdArea)
        {
            objInv[holdingNum] = pickUp;

            pickUp.GetComponent<Collider>().enabled = false;
            //Set out object to be parented to where we want it to go after holding it
            pickUp.transform.SetParent(holdArea);
            //idk this is a quality of life thing for me I just like it
            pickUp.transform.SetSiblingIndex(holdingNum);

            //Make sure that the rotations, positions, and scale match to what we want when picking it up
            setPosFunc(pickUp);

            //Set the layer to holding for the camera
            pickUp.layer = LayerMask.NameToLayer("Holding");
        }

        //**************************************************************************************************************

        void dropSystemFunc(IPlace placeObj)
        {
            if (pickUpObj != null)
            {
                //Get what we're doing with the interactable
                description.text = placeObj.getDescriptionFunc(pickUpObj);

                //if we click it run the place script
                if (Input.GetKeyDown(KeyCode.Mouse0)) dropFunc(placeObj.getPlaceAreaFunc());
            }
        }

        //**************************************************************************************************************

        void dropFunc(Transform placeArea)
        {
            pickUpObj.GetComponent<Collider>().enabled = true;

            //Set the parent to where we want to place it
            pickUpObj.transform.SetParent(placeArea);

            //Make sure that the rotations, positions, and scale match to what we want when placing it

            setPosFunc(pickUpObj);

            foreach (Transform transform in pickUpObj.transform)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }

            //Make the layer the default, enviroment layer
            pickUpObj.layer = LayerMask.NameToLayer("Default");

            pickUpObj.transform.SetParent(null);

            //remove the object from our inventory
            objInv[holdingNum] = null;
        }

        void setPosFunc(GameObject obj)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        //**************************************************************************************************************

        void handleGunFunc()
        {
            if (pickUpObj != null && pickUpObj.TryGetComponent(out GunClass gun))
            {
                crossHair = gun.crossGetter;

                if (hitSomething)
                {
                    crossHair.SetActive(false);
                    GunClass.crossSize = 0;
                    cursor.SetActive(true);

                } else{

                    crossHair.SetActive(true);
                    cursor.SetActive(false);
                }

                GunClass.showAmmo = true;
                GunClass.ammo = objAnim.GetCurrentAnimatorStateInfo(0).IsName("reload") ? "Reloading" : gun.gunMag.ToString() + "/" + gun.totalAmmo.ToString();


            } else {

                GunClass.showAmmo = false;
                if (crossHair != null)
                {
                    crossHair.SetActive(false); 
                    crossHair = null;

                    if (!cursor.activeSelf) cursor.SetActive(true);
                }

                GunClass.camCurrentRot = cam.transform.localRotation;
                GunClass.camTargetRot = Vector3.zero;
            }
        }
    }
}
