using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class interactDoor : MonoBehaviour {
    public bool isOpen = false;
    public bool isMoving = false;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float distance = 4f;

    private Vector3 startingPosition;
    private Vector3 forward;

    private Coroutine animationCoroutine;

    public void Awake()
    {
        startingPosition = transform.localPosition;

        forward = transform.right;
    }

    public void open()
    {
        if (!isOpen){
            if (animationCoroutine != null){
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(doOpen());
        }
    }

    private IEnumerator doOpen()
    {

        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = transform.localPosition += Vector3.up * distance;

        float time = 0;
        isMoving = true;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        if (transform.localPosition.y != 6)
            transform.localPosition = new Vector3(transform.localPosition.x, distance, transform.localPosition.z);

        yield return new WaitForEndOfFrame();
        isOpen = true;
        isMoving = false;

        yield return new WaitForSeconds(3f);

        startPosition = transform.localPosition;
        endPosition = transform.localPosition += -Vector3.up * distance;

        time = 0;
        isMoving = true;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        if (transform.localPosition.y != 0)
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

        yield return new WaitForEndOfFrame();
        isOpen = false;
        isMoving = false;
    }



    public void close()
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(doClose());
        }
    }

    public IEnumerator doClose()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startingPosition;

        float time = 0;
        isMoving = true;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
        
        if (transform.localPosition.y != 0)
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);


        yield return new WaitForEndOfFrame();
        isOpen = false;
        isMoving = false;
    }
}
