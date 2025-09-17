using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
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
            var body = hit.collider.attachedRigidbody;
            if (
                !collisionConfig.IsPushEnabled
                || body == null
                || body.isKinematic
                || hit.moveDirection.y < -0.3f
            )
                return;

            var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

            var pushStrength = Mathf.Clamp(
                collisionConfig.PushPower * controller.velocity.magnitude,
                0,
                collisionConfig.MaxPushForce
            );

            if (collisionConfig.UseForceInsteadOfVelocity)
                body.AddForce(pushDir * pushStrength, collisionConfig.forceMode);
            else
                body.linearVelocity = pushDir * pushStrength;
        }
    }
}
