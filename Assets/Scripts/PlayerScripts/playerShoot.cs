using UnityEngine;

//rename this to someting else
namespace player
{
    public class playerShoot : MonoBehaviour
    {
        private interactController holdObj;

        private void Awake() => holdObj = FindObjectOfType<interactController>();
    

        void Update()
        {
            if (holdObj.pickUpObj != null)
            {
                //See if it has the functioning interface. If it does, run it's thing every frame
                if (holdObj.pickUpObj.TryGetComponent(out IFunction functionThing))
                {
                    functionThing.doThisFunc();
                }
            }
        }
    }
}