using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pickUpController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance;

    [SerializeField] private TextMeshProUGUI HoldText;

    [SerializeField] private CharacterController playerVelocity;
    [SerializeField] private Transform holdArea;
    [SerializeField] private Transform enviroment;

    private GameObject pickUpObj = null;
    private Rigidbody objBody = null;
    private bool canPickUp = true;

    void Update()
    {
        if (canPickUp){pickUpObj = null; objBody = null;}

        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.TryGetComponent<IHold>(out IHold holdObj))
            {
                hitSomething = true;

                Debug.Log(holdObj.getDescription());
                HoldText.text = holdObj.getDescription();

                if (objBody == null)objBody = hit.transform.GetComponent<Rigidbody>();
                if (pickUpObj == null) pickUpObj = hit.collider.gameObject;
            }

        }if (!hitSomething) HoldText.text = "";

        //Picking up an object
        if (pickUpObj != null && Input.GetKeyDown(KeyCode.E) && canPickUp) pickUp();

        //Dropping an object
        if (Input.GetKeyDown(KeyCode.Q) && !canPickUp) drop();
    }


    void pickUp()
    {
        pickUpObj.transform.SetParent(holdArea);

        pickUpObj.transform.localPosition = Vector3.zero;
        pickUpObj.transform.localEulerAngles = Vector3.zero;
        pickUpObj.transform.localScale = Vector3.one;

        objBody.isKinematic = true;

        canPickUp = false;

    }


    void drop()
    {
        pickUpObj.transform.SetParent(enviroment);

        pickUpObj.transform.localScale = Vector3.one;

        objBody.velocity = playerVelocity.velocity;

        objBody.isKinematic = false;
        objBody.AddForce(cam.transform.forward * 1.5f, ForceMode.Impulse);
        objBody.AddForce(cam.transform.up * 2f, ForceMode.Impulse);

        float randomRange = Random.Range(-1, 1);
        objBody.AddTorque(new Vector3(1, randomRange, randomRange) * 10);

        canPickUp = true;
    }
}
