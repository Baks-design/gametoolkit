using Alchemy.Inspector;
using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Systems.UpdateManagement;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerCollisionController : CustomMonoBehaviour
    {
        [SerializeField]
        CharacterController controller;

        [SerializeField]
        PlayerCollisionConfig collisionConfig;

        [SerializeField, ReadOnly]
        PlayerCollisionData collisionData;

        readonly PlayerMovementData movementData = new();

        protected override void Awake()
        {
            controller.center = new Vector3(0f, controller.height / 2f + controller.skinWidth, 0f);
            collisionData = new PlayerCollisionData
            {
                InitCenter = controller.center,
                InitHeight = controller.height,
                OnGrounded = true,
                PreviouslyGrounded = true,
                FinalRayLength = collisionConfig.RayLength + controller.center.y
            };
        }

        public override void ProcessUpdate(float deltaTime)
        {
            CheckIfGrounded();
            CheckIfWall();
            CheckIfRoof();
            collisionData.PreviouslyGrounded = collisionData.OnGrounded;
        }

        void CheckIfGrounded()
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

        void CheckIfWall()
        {
            if (!InputManager.HasMovement || movementData.FinalMoveDirection.sqrMagnitude <= 0f)
                return;

            var hitWall = Physics.SphereCast(
                controller.transform.position + controller.center,
                collisionConfig.RayObstacleSphereRadius,
                movementData.FinalMoveDirection,
                out var wallInfo,
                collisionConfig.RayObstacleLength,
                collisionConfig.ObstacleLayers
            );

            collisionData.HasObstructed = hitWall;
            collisionData.ObstructedNormal = wallInfo.normal;
        }

        void CheckIfRoof()
        {
            var hitRoof = Physics.SphereCast(
                controller.transform.position,
                collisionData.RoofRaySphereRadius,
                Vector3.up,
                out var _,
                collisionData.InitHeight
            );

            collisionData.HasRoofed = hitRoof;
        }
    }
}
