using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class GroundCheck
    {
        readonly CharacterController controller;
        readonly PlayerCollisionConfig collisionConfig;
        readonly PlayerCollisionData collisionData;
        readonly RaycastHit[] groundHits = new RaycastHit[1];

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
            var sphereOrigin = controller.transform.position + controller.center;
            var rayLength = collisionData.FinalRayLength;
            var hitCount = Physics.SphereCastNonAlloc(
                sphereOrigin,
                collisionConfig.RaySphereRadius,
                Vector3.down,
                groundHits,
                rayLength,
                collisionConfig.GroundLayer,
                QueryTriggerInteraction.Ignore
            );
            var hitGround = hitCount > 0;
            var hitInfo = hitGround ? groundHits[0] : default;

            collisionData.OnGrounded = hitGround;
            collisionData.CastHit = hitInfo;
            collisionData.GroundedNormal = hitInfo.normal;
        }
    }
}
