using UnityEngine;

public class Interact : MonoBehaviour, Interactable
{
    private float downTime;
    public string getDescriptionFunc()
    {
        return "[E] Pick Up";
    }

    public bool getDownTimeFunc()
    {
        if (downTime + 0.5f < Time.time) return false;
        else return true;
    }

    public void interactFunc()
    {
        Debug.Log("working");
        downTime = Time.time;
    }

    Interactable.InteractionType Interactable.getTypeFunc()
    {
        return Interactable.InteractionType.Hold;
    }
}
