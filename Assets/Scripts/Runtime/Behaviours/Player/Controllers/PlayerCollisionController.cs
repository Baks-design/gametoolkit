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

        [HideInInspector, SerializeField]
        PlayerMovementConfig movementConfig;

        PlayerCollisionData collisionData;
        PlayerMovementData movementData;
        PlayerInputData inputData;

        protected override void Awake()
        {
            base.Awake();
            SetupComponents();
        }

        void SetupComponents()
        {
            controller.center = new Vector3(0f, controller.height / 2f + controller.skinWidth, 0f);
            collisionData.InitCenter = controller.center;
            collisionData.InitHeight = controller.height;
            collisionData.OnGrounded = true;
            collisionData.PreviouslyGrounded = true;
            collisionData.FinalRayLength = collisionConfig.RayLength + controller.center.y;
        }

        public override void ProcessUpdate(float deltaTime)
        {
            base.ProcessUpdate(deltaTime);
            CheckIfGrounded();
            CheckIfWall();
            CheckIfRoof();
            collisionData.PreviouslyGrounded = collisionData.OnGrounded;
        }

        void CheckIfGrounded()
        {
            var hitGround = Physics.SphereCast(
                transform.position + controller.center,
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
            if (!inputData.HasMoveInput || movementData.FinalMoveDirection.sqrMagnitude <= 0f)
                return;

            var hitWall = Physics.SphereCast(
                Transform.position + controller.center,
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
                Transform.position,
                collisionData.RoofRaySphereRadius,
                Vector3.up,
                out var _,
                collisionData.InitHeight
            );

            collisionData.HasRoofed = hitRoof;
        }
    }
}
