using UnityEngine.Animations.Rigging;
using UnityEngine;
using player;
using TMPro;

public class holdObjManager : MonoBehaviour
{
    [Header("Ammo Count")]
    [SerializeField] protected TextMeshProUGUI ammoCount;

    [Header("Arms")]
    [SerializeField] private GameObject arms;
    [SerializeField] private GameObject leftHandle;
    [SerializeField] private GameObject rightHandle;
    [SerializeField] private GameObject leftHandleHint;
    [SerializeField] private GameObject rightHandleHint;


    [Header("CrossHair")]
    [SerializeField] private GameObject cursor;
    private GameObject crossHair;

    [Header("Interact Controller")]
    private interactController interactController;
    private GameObject pickUpObj;
    private Animator objAnim;

    [Header("Cam")]
    [SerializeField] private Camera cam;

    void Start()
    {
        interactController = FindObjectOfType<interactController>();
    }

    // Update is called once per frame
    void Update()
    {
        pickUpObj = interactController.pickUpObj;

        if (pickUpObj != null)
        {
            objAnim = pickUpObj.GetComponentInChildren<Animator>();
            var selectedObj = pickUpObj.GetComponent<IPick>();

            leftHandle.transform.position = selectedObj.getLeftHandTargetFunc().position;
            leftHandle.transform.rotation = selectedObj.getLeftHandTargetFunc().rotation;
            rightHandle.transform.position = selectedObj.getRightHandTargetFunc().position;
            rightHandle.transform.rotation = selectedObj.getRightHandTargetFunc().rotation;

            leftHandleHint.transform.position = selectedObj.getLeftHandHintFunc().position;
            leftHandleHint.transform.rotation = selectedObj.getLeftHandHintFunc().rotation;
            rightHandleHint.transform.position = selectedObj.getRightHandHintFunc().position;
            rightHandleHint.transform.rotation = selectedObj.getRightHandHintFunc().rotation;

            arms.SetActive(true);

            if (pickUpObj.TryGetComponent(out GunClass gun))
            {
                crossHair = gun.crossGetter;

                if (interactController.hitSomething)
                {
                    crossHair.SetActive(false);
                    GunClass.crossSize = 0;
                    cursor.SetActive(true);

                } else {

                    crossHair.SetActive(true);
                    cursor.SetActive(false);
                }

                ammoCount.enabled = true;
                ammoCount.SetText(objAnim.GetCurrentAnimatorStateInfo(0).IsName("reload") ? "Reloading" : gun.gunMag.ToString() + "/" + gun.totalAmmo.ToString());
            }

        } else {

            arms.SetActive(false);

            ammoCount.enabled = false;

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
