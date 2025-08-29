using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAiming : MonoBehaviour
    {
        [Header("References")]
        public Transform bodyTransform;

        [Header("Sensitivity")]
        public float sensitivityMultiplier = 1f;
        public float horizontalSensitivity = 1f;
        public float verticalSensitivity = 1f;

        [Header("Restrictions")]
        public float minYRotation = -90f;
        public float maxYRotation = 90f;

        //The real rotation of the camera without recoil
        Vector3 realRotation;

        [Header("Aimpunch")]
        [Tooltip(
            "bigger number makes the response more damped, smaller is less damped, currently the system will overshoot, with larger damping values it won't"
        )]
        public float punchDamping = 9f;

        [Tooltip("bigger number increases the speed at which the view corrects")]
        public float punchSpringConstant = 65f;

        [HideInInspector]
        public Vector2 punchAngle;

        [HideInInspector]
        public Vector2 punchAngleVel;

        void Start()
        {
            // Lock the mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            // Fix pausing
            if (Mathf.Abs(Time.timeScale) <= 0f)
                return;

            DecayPunchAngle();

            // Input
            var xMovement =
                Input.GetAxisRaw("Mouse X") * horizontalSensitivity * sensitivityMultiplier;
            var yMovement =
                -Input.GetAxisRaw("Mouse Y") * verticalSensitivity * sensitivityMultiplier;

            // Calculate real rotation from input
            realRotation = new Vector3(
                Mathf.Clamp(realRotation.x + yMovement, minYRotation, maxYRotation),
                realRotation.y + xMovement,
                realRotation.z
            );
            realRotation.z = Mathf.Lerp(realRotation.z, 0f, Time.deltaTime * 3f);

            //Apply real rotation to body
            bodyTransform.eulerAngles = Vector3.Scale(realRotation, new Vector3(0f, 1f, 0f));

            //Apply rotation and recoil
            var cameraEulerPunchApplied = realRotation;
            cameraEulerPunchApplied.x += punchAngle.x;
            cameraEulerPunchApplied.y += punchAngle.y;

            transform.eulerAngles = cameraEulerPunchApplied;
        }

        public void ViewPunch(Vector2 punchAmount)
        {
            //Remove previous recoil
            punchAngle = Vector2.zero;
            //Recoil go up
            punchAngleVel -= punchAmount * 20f;
        }

        void DecayPunchAngle()
        {
            if (punchAngle.sqrMagnitude > 0.001f || punchAngleVel.sqrMagnitude > 0.001f)
            {
                punchAngle += punchAngleVel * Time.deltaTime;
                var damping = 1f - (punchDamping * Time.deltaTime);

                if (damping < 0f)
                    damping = 0f;

                punchAngleVel *= damping;

                var springForceMagnitude = punchSpringConstant * Time.deltaTime;
                punchAngleVel -= punchAngle * springForceMagnitude;
            }
            else
            {
                punchAngle = Vector2.zero;
                punchAngleVel = Vector2.zero;
            }
        }
    }
}
