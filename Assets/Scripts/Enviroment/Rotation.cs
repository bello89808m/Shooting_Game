using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float z = Mathf.PingPong(Time.time, 10);
        transform.Rotate(new Vector3(1, 1, z) * 5 * Time.deltaTime);
    }
}
