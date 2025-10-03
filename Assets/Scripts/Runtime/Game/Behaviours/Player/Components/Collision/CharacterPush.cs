using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class CharacterPush
    {
        readonly CharacterController controller;
        readonly PlayerCollisionData collisionData;
        readonly PlayerCollisionConfig collisionConfig;

        public CharacterPush(
            CharacterController controller,
            PlayerCollisionData collisionData,
            PlayerCollisionConfig collisionConfig
        )
        {
            this.controller = controller;
            this.collisionData = collisionData;
            this.collisionConfig = collisionConfig;
        }

        public void PushBody(ControllerColliderHit hit)
        {
            collisionData.HasObjectColliding = hit.collider.isTrigger;

            if (!collisionConfig.IsPushEnabled || hit.moveDirection.y < -0.3f)
                return;

            var body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            if (pushDir.sqrMagnitude < 0.001f)
                return;
            pushDir.Normalize();

            var controllerSpeed = controller.velocity.magnitude;
            var pushStrength = Mathf.Clamp(
                collisionConfig.PushPower * controllerSpeed,
                0f,
                collisionConfig.MaxPushForce
            );
            if (pushStrength < 0.001f)
                return;

            if (collisionConfig.UseForceInsteadOfVelocity)
                body.AddForce(pushDir * pushStrength, collisionConfig.forceMode);
            else
                body.linearVelocity = pushDir * pushStrength;
        }
    }
}
