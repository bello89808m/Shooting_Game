using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace player
{
    public class playerShoot : MonoBehaviour
    {            
        void Update()
        {
            if(FindObjectOfType<interactController>().pickUpObj != null)
            {
                if (FindObjectOfType<interactController>().pickUpObj.TryGetComponent<IFunction>(out IFunction functionThing) && functionThing.canFunc())
                {
                    functionThing.doThis();
                }
            }
        }
    }
}