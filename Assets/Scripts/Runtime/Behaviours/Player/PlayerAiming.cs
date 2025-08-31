using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAiming : CustomMonoBehaviour
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

        [Header("Aimpunch")]
        [Tooltip(
            "Bigger number makes the response more damped, smaller is less damped."
                + "Currently the system will overshoot, with larger damping values it won't"
        )]
        public float punchDamping = 9f;

        [Tooltip("Bigger number increases the speed at which the view corrects")]
        public float punchSpringConstant = 65f;

        [HideInInspector]
        public Vector2 punchAngle;

        [HideInInspector]
        public Vector2 punchAngleVel;

        Vector3 realRotation;
        const float MouseInputScale = 20f;
        const float ZRotationResetSpeed = 3f;
        const float Epsilon = 0.001f;

        public override void ProcessLateUpdate(float deltaTime)
        {
            base.ProcessLateUpdate(deltaTime);
            if (ShouldSkipUpdate())
                return;
            DecayPunchAngle(deltaTime);
            HandleMouseInput(deltaTime);
            ApplyRotations();
        }

        bool ShouldSkipUpdate() => Mathf.Abs(Time.timeScale) <= 0f;

        void HandleMouseInput(float deltaTime)
        {
            var xMovement = UpdateLookInput().x * horizontalSensitivity * sensitivityMultiplier;
            var yMovement = -UpdateLookInput().y * verticalSensitivity * sensitivityMultiplier;
            UpdateRealRotation(xMovement, yMovement, deltaTime);
        }

        Vector2 UpdateLookInput() => Mouse.current.delta.ReadDefaultValue();

        void UpdateRealRotation(float xMovement, float yMovement, float deltaTime)
        {
            realRotation.x = Mathf.Clamp(realRotation.x + yMovement, minYRotation, maxYRotation);
            realRotation.y += xMovement;
            realRotation.z = Mathf.Lerp(realRotation.z, 0f, deltaTime * ZRotationResetSpeed);
        }

        void ApplyRotations()
        {
            ApplyBodyRotation();
            ApplyCameraRotation();
        }

        void ApplyBodyRotation() => bodyTransform.eulerAngles = new Vector3(0f, realRotation.y, 0f);

        void ApplyCameraRotation()
        {
            var cameraEulerPunchApplied = realRotation;
            cameraEulerPunchApplied.x += punchAngle.x;
            cameraEulerPunchApplied.y += punchAngle.y;
            Transform.eulerAngles = cameraEulerPunchApplied;
        }

        public void ViewPunch(Vector2 punchAmount)
        {
            punchAngle = Vector2.zero;
            punchAngleVel -= punchAmount * MouseInputScale;
        }

        void DecayPunchAngle(float deltaTime)
        {
            if (punchAngle.sqrMagnitude > Epsilon || punchAngleVel.sqrMagnitude > Epsilon)
                UpdatePunchDynamics(deltaTime);
            else
                ResetPunch();
        }

        void UpdatePunchDynamics(float deltaTime)
        {
            punchAngle += punchAngleVel * deltaTime;

            var damping = Mathf.Max(1f - (punchDamping * deltaTime), 0f);
            punchAngleVel *= damping;

            var springForceMagnitude = punchSpringConstant * deltaTime;
            punchAngleVel -= punchAngle * springForceMagnitude;
        }

        void ResetPunch()
        {
            punchAngle = Vector2.zero;
            punchAngleVel = Vector2.zero;
        }
    }
}
