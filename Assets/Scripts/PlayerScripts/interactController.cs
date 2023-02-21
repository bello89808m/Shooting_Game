using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class interactController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float distance = .0f;

    [Header("Cursor")]
    [SerializeField] private TextMeshProUGUI InteractionText;
    [SerializeField] private GameObject InteractingHoldGo;
    [SerializeField] private Image interactionProgress;

    [Header("Hold")]
    [SerializeField] private CharacterController playerVelocity;
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform enviroment;
    private GameObject pickUpObj = null;
    private Rigidbody objBody = null;
    private bool canPickUp = true;

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;

        //If an object is not picked up, set these to null
        if (canPickUp)
        {
            pickUpObj = null;
            objBody = null;
        }

        if (Physics.Raycast(ray, out hit, distance))
        {
            //If you want to interact with an object
            Interactable interacting = hit.collider.GetComponent<Interactable>();

            if (interacting != null && interacting.canInteract)
            {
                HandleInteraction(interacting);

                hitSomething = true;

                InteractionText.text = interacting.getDescription();

                InteractingHoldGo.SetActive(interacting.interactiontype == Interactable.InteractionType.Hold);

            //If the object wants to be picked up
            } else if(hit.collider.TryGetComponent<IHold>(out IHold holdObj)){

                hitSomething = true;

                InteractionText.text = holdObj.getDescription();

                if(objBody == null){
                    hit.transform.TryGetComponent<Rigidbody>(out Rigidbody objBody);
                    this.objBody = objBody;
                }

                if(pickUpObj == null) pickUpObj = hit.collider.gameObject;
            }

        //When the raycast isn't hitting anything
        }if (!hitSomething){
            InteractionText.text = "";
            InteractingHoldGo.SetActive(false);
        }


        //Picking up an object
        if(pickUpObj != null && Input.GetKeyDown(KeyCode.E) && canPickUp){
            pickUp();

        //Dropping an object
        } if(Input.GetKeyDown(KeyCode.Q) && !canPickUp) {
            drop();
        }
    }


    void HandleInteraction(Interactable interactable)
    {
        KeyCode Key = interactable.definedKey();

        switch (interactable.interactiontype)
        {
            //If we want the type to be click
            case Interactable.InteractionType.Click:
                if (Input.GetKeyDown(Key))
                {
                    interactable.interact();
                }
                break;

            //If we want the type to be hold
            case Interactable.InteractionType.Hold:
                if (Input.GetKey(Key))
                {
                    interactable.holdingTime();

                    if (interactable.getHoldTime() > 1.0f)
                    {
                        interactable.interact();
                        interactable.resetTime();
                    }

                }
                else
                {
                    interactable.resetTime();
                }

                interactionProgress.fillAmount = interactable.getHoldTime();
                break;

            //When the result is null
            default:
                throw new System.Exception("Unsupported Interactable");
        }
    }


    void pickUp()
    {
        pickUpObj.transform.SetParent(holdArea);

        pickUpObj.transform.localPosition = Vector3.zero;
        pickUpObj.transform.localEulerAngles = Vector3.zero;
        pickUpObj.transform.localScale = Vector3.one;

        if (objBody != null)
        {
            objBody.isKinematic = true;
        }

        canPickUp = false;

    }


    void drop()
    {
        pickUpObj.transform.SetParent(enviroment);

        pickUpObj.transform.localScale = Vector3.one;

        if (objBody != null)
        {
            objBody.velocity = playerVelocity.velocity;


            objBody.isKinematic = false;
            objBody.AddForce(cam.transform.forward * 8.5f, ForceMode.Impulse);
            objBody.AddForce(cam.transform.up * 2f, ForceMode.Impulse);

            float randomRange = Random.Range(-1, 1);
            objBody.AddTorque(new Vector3(1, randomRange, randomRange) * 10);
        }

        canPickUp = true;
    }
}
