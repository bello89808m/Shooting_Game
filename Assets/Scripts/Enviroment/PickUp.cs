using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    [SerializeField] private Transform boxholder;
    [SerializeField] private Transform enviroment;

    [SerializeField] private Collider boxCol;
    [SerializeField] private Rigidbody boxBody;

    private Vector3 startPosition;
    private Coroutine pickingUp;

    private bool isOccupied = false;

    public override string getDescription()
    {
        if (!isOccupied){
            return "[E] Pick Up";
        }
        return "";
    }

    public override void interact()
    {
        if (!isOccupied){
            transform.SetParent(boxholder);

            startPosition = transform.localPosition;

            pickingUp = StartCoroutine(doPickUp());
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;

            boxBody.useGravity = false;
            boxBody.isKinematic = true;
            boxCol.enabled = true;

            canInteract = false;
            isOccupied = true;
        }
    }

    public void Update()
    {
        if (isOccupied && Input.GetKeyDown(KeyCode.Q))
        {
            transform.SetParent(enviroment);

            transform.localScale = Vector3.one;
            if (pickingUp != null)
                StopCoroutine(pickingUp);

            boxBody.useGravity = true;
            boxBody.isKinematic = false;
            boxCol.enabled = false;

            canInteract = true;
            isOccupied = false;
        }
    }

    public IEnumerator doPickUp()
    {
        float time = 0;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, time * 7);
            yield return null;
            time += Time.deltaTime;
        }

        if(transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
