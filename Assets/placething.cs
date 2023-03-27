using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placething : MonoBehaviour,IPlace
{
    public string getDescription(GameObject name)
    {
        return "";
    }

    public Transform getPlaceArea()
    {
        return this.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
