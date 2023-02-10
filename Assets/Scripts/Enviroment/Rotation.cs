using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : Interactable
{
    public override string getDescription()
    {
        return "Destroy?";
    }

    public override void interact()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        float z = Mathf.PingPong(Time.time, 10);
        transform.Rotate(new Vector3(1, 1, z) * 10 * Time.deltaTime);
    }
}
