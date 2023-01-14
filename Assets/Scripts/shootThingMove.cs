using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootThingMove : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float speed;
    private Vector3 startPos;

    // Start is called before the first frame update
    private void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = startPos;
        v.x += distance * Mathf.Sin(Time.time*speed);
        transform.position = v;
    }
}
