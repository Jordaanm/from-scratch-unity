using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSCamera
{
    public class CameraControls : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        public bool invertVerticalMovement;
        private @FromScratchControls fromScratchControls;
        private InputAction lookAction;
        private InputAction moveAction;

        public float rotationPower = 3f;
        private float aimValue = 0; //TODO: Look Ahead button mapping
        private float fireValue = 0; //TODO: Primary Action button mapping
        private void Awake()
        {
            fromScratchControls = new FromScratchControls();
            lookAction = fromScratchControls.Player.Look;
            moveAction = fromScratchControls.Player.Move;
        }

        private void OnEnable()
        {
            lookAction.Enable();
            moveAction.Enable();
        }

        private void OnDisable()
        {
            lookAction.Disable();
            moveAction.Disable();
        }

        private void Update()
        {
            Transform followTransform = virtualCamera.Follow.transform;
            Vector2 look = lookAction.ReadValue<Vector2>();
            Vector3 move = moveAction.ReadValue<Vector2>();

            //Horizontal Rotation
            followTransform.rotation *= Quaternion.AngleAxis(look.x * rotationPower * Time.deltaTime, Vector3.up);

            //Vertical Rotation
            float inversion = invertVerticalMovement ? -1 : 1;
            followTransform.rotation *= Quaternion.AngleAxis(look.y * inversion * rotationPower * Time.deltaTime, Vector3.right);

            var angles = followTransform.localEulerAngles;
            angles.z = 0;
            var angle = followTransform.localEulerAngles.x;
            //Clamp the Up/Down rotation
            if (angle > 180 && angle < 340)
            {
                angles.x = 340;
            } else if (angle < 180 && angle > 40)
            {
                angles.x = 40;
            }
            followTransform.transform.localEulerAngles = angles;
            
            if (move.x == 0 && move.y == 0)
            {
                if (Mathf.Approximately(aimValue, 1f))
                {
                    //reset the y rotation of the look transform
                    followTransform.localEulerAngles = new Vector3(angles.x, 0, 0);
                }
            }
        }
    }
}
