using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        float z = Mathf.PingPong(Time.time, 5);
        float y = Mathf.Sin(Time.time * 3f) * 0.001f;

        transform.Rotate(new Vector3(1, 1, z) * 10 * Time.deltaTime);
        transform.position += new Vector3(0, y, 0);
    }
}
