using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoldController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] private float distance = 2.0f;

    [SerializeField] private TextMeshProUGUI InteractionText;

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;
        bool hitSomething = false;
        if(Physics.Raycast(ray, out hit, distance))
        {
            Holding holdable = hit.collider.GetComponent<Holding>();

            if (holdable != null)
            {
                checkType(holdable);
            }
            
        }
    }

    public void checkType(Holding holdInteract)
    {

    }
}
