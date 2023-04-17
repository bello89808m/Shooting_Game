using UnityEngine;

namespace player
{
    public class cameraBob : MonoBehaviour
    {
        [SerializeField] private new Transform camera = null;
        [SerializeField] private playerMovement playerSpeed;
        [SerializeField] private CharacterController controller;

        private Vector3 startPos;


        // Start is called before the first frame update
        private void Awake() => startPos = camera.localPosition;


        // Update is called once per frame
        private void Update() => CheckMotionFunc();

        private void PlayMotionFunc(Vector3 motion) => camera.localPosition += motion;

        private void CheckMotionFunc()
        {
            ResetPositionFunc();
            setBobSpeedFunc();
        }

        private void setBobSpeedFunc()
        {
            switch (playerSpeed.state)
            {
                case playerMovement.movementState.walking:
                    PlayMotionFunc(FootStepsFunc(6f, 0.002f));
                    break;
                case playerMovement.movementState.sprinting:
                    PlayMotionFunc(FootStepsFunc(8f, 0.0025f));
                    break;
                case playerMovement.movementState.crouching:
                    PlayMotionFunc(FootStepsFunc(4f, 0.0015f));
                    break;
                default:
                    PlayMotionFunc(FootStepsFunc(0f, 0f));
                    break;
            }
        }

        private Vector3 FootStepsFunc(float frequency, float amplitude)
        {
            //Create a new vector thats 0,0,0
            Vector3 pos = Vector3.zero;
            //the new vectors y value will do this math and go up and down depending on these two variables. I dont know how Sin works yet and I'm too afraid to find out now
            pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
            return pos;
        }

        private void ResetPositionFunc()
        {
            if (camera.localPosition == startPos) return;

            camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 5 * Time.deltaTime);
        }
    }
}
