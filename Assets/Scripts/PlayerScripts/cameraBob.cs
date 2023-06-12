using UnityEngine;

namespace player
{
    public class cameraBob : MonoBehaviour
    {
        [SerializeField] private new Transform camera = null;
        [SerializeField] private playerMovement playerSpeed;
        [SerializeField] private interactController InteractController;


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
                    PlayMotionFunc(FootStepsFunc(10f, 0.000885f));
                    break;
                case playerMovement.movementState.sprinting:
                    PlayMotionFunc(FootStepsFunc(12f, 0.0011f));
                    break;
                default:
                    PlayMotionFunc(FootStepsFunc(4f, 0.00071f));
                    break;
            }
        }

        private Vector3 FootStepsFunc(float frequency, float amplitude)
        {
            //Create a new vector thats 0,0,0
            Vector3 pos = Vector3.zero;
            //the new vectors y value will do this math and go up and down depending on these two variables. I dont know how Sin works yet and I'm too afraid to find out now
            pos.y += Mathf.Sin(Time.time * frequency) * amplitude / 1.35f;
            pos.x += Mathf.Sin(Time.time * frequency/2) * amplitude / 1.4f;

            return pos;
        }

        private void ResetPositionFunc()
        {
            if (camera.localPosition == startPos) return;

            camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 5 * Time.deltaTime);
        }
    }
}
