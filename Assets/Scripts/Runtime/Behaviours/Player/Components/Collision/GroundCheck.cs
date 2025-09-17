using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class GroundCheck
    {
        readonly CharacterController controller;
        readonly PlayerCollisionConfig collisionConfig;
        readonly PlayerCollisionData collisionData;

        public GroundCheck(
            CharacterController controller,
            PlayerCollisionConfig collisionConfig,
            PlayerCollisionData collisionData
        )
        {
            this.controller = controller;
            this.collisionConfig = collisionConfig;
            this.collisionData = collisionData;
        }

        public void CheckGround()
        {
            var hitGround = Physics.SphereCast(
                controller.transform.position + controller.center,
                collisionConfig.RaySphereRadius,
                Vector3.down,
                out var hitInfo,
                collisionData.FinalRayLength,
                collisionConfig.GroundLayer
            );

            collisionData.OnGrounded = hitGround;
            collisionData.CastHit = hitInfo;
            collisionData.GroundedNormal = hitInfo.normal;
        }
    }
}
