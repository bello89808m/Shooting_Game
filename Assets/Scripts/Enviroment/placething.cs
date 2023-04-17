using UnityEngine;

public class placething : MonoBehaviour,IPlace
{
    public string getDescriptionFunc(GameObject name)
    {
        return "";
    }

    public Transform getPlaceAreaFunc()
    {
        return transform;
    }
}
