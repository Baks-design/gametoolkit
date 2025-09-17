using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class ObstacleCheck
    {
        readonly CharacterController controller;
        readonly PlayerCollisionConfig collisionConfig;
        readonly PlayerMovementData movementData;
        readonly PlayerCollisionData collisionData;

        public ObstacleCheck(
            CharacterController controller,
            PlayerCollisionConfig collisionConfig,
            PlayerMovementData movementData,
            PlayerCollisionData collisionData
        )
        {
            this.controller = controller;
            this.collisionConfig = collisionConfig;
            this.movementData = movementData;
            this.collisionData = collisionData;
        }

        public void CheckObstacle()
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
    }
}
