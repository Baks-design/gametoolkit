using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CharacterPush
    {
        readonly CharacterController controller;
        readonly PlayerCollisionConfig collisionConfig;

        public CharacterPush(CharacterController controller, PlayerCollisionConfig collisionConfig)
        {
            this.controller = controller;
            this.collisionConfig = collisionConfig;
        }

        public void PushBody(ControllerColliderHit hit)
        {
            // Early exit conditions
            if (!collisionConfig.IsPushEnabled || hit.moveDirection.y < -0.3f)
                return;

            var body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            // Calculate push direction (horizontal only)
            var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            // Early exit if push direction is negligible
            if (pushDir.sqrMagnitude < 0.001f)
                return;

            // Normalize direction for consistent force application
            pushDir.Normalize();

            // Calculate push strength
            var controllerSpeed = controller.velocity.magnitude;
            var pushStrength = Mathf.Clamp(
                collisionConfig.PushPower * controllerSpeed,
                0f,
                collisionConfig.MaxPushForce
            );
            // Early exit if push strength is negligible
            if (pushStrength < 0.001f)
                return;

            // Apply push force
            if (collisionConfig.UseForceInsteadOfVelocity)
                body.AddForce(pushDir * pushStrength, collisionConfig.forceMode);
            else
                body.linearVelocity = pushDir * pushStrength;
        }
    }
}
