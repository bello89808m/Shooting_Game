using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class interactDoor : MonoBehaviour {
    public bool isOpen = false;
    public bool isMoving = false;

    private float speed = 1f;
    private float distance = 4f;

    private Vector3 startingPosition;

    private Coroutine animationCoroutine;

    public void Awake()
    {
        //when activated, have the starting position be the default position
        startingPosition = transform.localPosition;
    }

    //**************************************************************************************************************

    public void open()
    {
        //When the door is not open, stop the animation if another on is running then start the open coroutine
        if (!isOpen){
            if (animationCoroutine != null){
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(doOpen());
        }
    }

    //**************************************************************************************************************

    private IEnumerator doOpen()
    {
        //self explanatory
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = transform.localPosition += Vector3.up * distance;

        float time = 0;
        isMoving = true;

        //while the time moving is less than 1
        while (time < 1)
        {
            //move it up on multiple framed
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            //add the time to the frames the game is running at multiplied by the speed we want it to be
            time += Time.deltaTime * speed;
        }
        //wait until the next frame before we can interact with it again
        yield return new WaitForEndOfFrame();
        isOpen = true;
        isMoving = false;

        //if the door is still open for 3 seconds
        yield return new WaitForSeconds(3f);

        //self-explanatory once a fucking again
        startPosition = transform.localPosition;
        endPosition = transform.localPosition += -Vector3.up * distance;

        //same thing as mentioned above but we're closing it
        time = 0;
        isMoving = true;

        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        yield return new WaitForEndOfFrame();
        isOpen = false;
        isMoving = false;
    }

    //**************************************************************************************************************

    //same thing as teh open functions
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

    //**************************************************************************************************************

    public IEnumerator doClose()
    {
        //same thing as the open function, but we want to starting position to be where we're at and the end position to be where we started
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startingPosition;

        float time = 0;
        isMoving = true;

        while (time < 1)
        {
            //for some fucking reason it wouldn't work when there was a start variable outside this coroutine but works when the start variable was in the coroutine. wtf happened?
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }


        yield return new WaitForEndOfFrame();
        isOpen = false;
        isMoving = false;
    }
}
