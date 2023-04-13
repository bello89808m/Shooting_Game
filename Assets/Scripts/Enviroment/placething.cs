using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placething : MonoBehaviour,IPlace
{
    public string getDescriptionFunc(GameObject name)
    {
        return "";
    }

    public Transform getPlaceAreaFunc()
    {
        return this.transform;
    }
}
