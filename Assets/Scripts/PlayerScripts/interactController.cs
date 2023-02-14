using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class interactController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance = 3.0f;

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
            Interactable interacting = hit.collider.GetComponent<Interactable>();

            if (interacting != null && interacting.canInteract)
            {
                HandleInteraction(interacting);

                hitSomething = true;
                
                InteractionText.text = interacting.getDescription();

                InteractingHoldGo.SetActive(interacting.interactiontype == Interactable.InteractionType.Hold);
            }
        }
        if (!hitSomething)
        {
            InteractionText.text = "";
            InteractingHoldGo.SetActive(false);
        }
    }

    void HandleInteraction(Interactable interactable)
    {
        KeyCode Key = interactable.definedKey();

        switch (interactable.interactiontype)
        {
            case Interactable.InteractionType.Click:
                if (Input.GetKeyDown(Key))
                {
                    interactable.interact();
                }
                break;

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

            default:
                throw new System.Exception("Unsupported Interactable");
        }
    }
}
