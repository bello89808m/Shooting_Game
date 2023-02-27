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
    [SerializeField] private TextMeshProUGUI InteractionText;
    [SerializeField] private GameObject InteractingHoldGo;
    [SerializeField] private Image interactionProgress;

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;
      

        if (Physics.Raycast(ray, out hit, distance))
        {
            //If you want to interact with an object
            Interactable interacting = hit.collider.GetComponent<Interactable>();

            if (interacting != null && interacting.canInteract)
            {
                //If we actually interact with it
                HandleInteraction(interacting);

                hitSomething = true;

                InteractionText.text = interacting.getDescription();

                InteractingHoldGo.SetActive(interacting.interactiontype == Interactable.InteractionType.Hold);
            }

        //When the raycast isn't hitting anything
        }if (!hitSomething){
            InteractionText.text = "";
            InteractingHoldGo.SetActive(false);
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
}
