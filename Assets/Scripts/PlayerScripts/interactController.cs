using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class interactController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance = 3.0f;

    public GameObject InteractionUI;
    public TextMeshProUGUI InteractionText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one/2f);
        RaycastHit hit;

        bool hitSomething = false;

        if(Physics.Raycast(ray, out hit, distance))
        {
            Interactable interacting = hit.collider.GetComponent<Interactable>();

            if(interacting != null){
                hitSomething = true;

                InteractionText.text = interacting.getDescription();

                if(Input.GetKeyDown(KeyCode.E)){
                    interacting.interact();
                }
            }
        }
        InteractionUI.SetActive(hitSomething);
    }
}
