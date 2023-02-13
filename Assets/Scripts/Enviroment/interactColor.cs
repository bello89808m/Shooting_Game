using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactColor : Interactable
{
    private MeshRenderer render;
    private Coroutine changeColor;
    public Material[] color = new Material[5];

    public void Start()
    {
        render = GetComponent<MeshRenderer>();
    }
    public override string getDescription()
    {
        return "Change Color";
    }

    public override void interact()
    {
        render.material = color[Random.Range(0,5)];
        canInteract = false;
    }
}
