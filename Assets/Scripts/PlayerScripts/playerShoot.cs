using UnityEngine;


namespace player
{
    public class playerShoot : MonoBehaviour
    {
        private interactController holdObj;

        private void Awake()
        {
            holdObj = FindObjectOfType<interactController>();
        }

        void Update()
        {
            if (holdObj.pickUpObj != null)
            {
                if (holdObj.pickUpObj.TryGetComponent(out IFunction functionThing))
                {
                    functionThing.doThisFunc();
                }
            }
        }
    }
}