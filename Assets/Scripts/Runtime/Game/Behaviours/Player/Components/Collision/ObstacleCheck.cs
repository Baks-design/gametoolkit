using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class ObstacleCheck
    {
        readonly IMovementInput movementInput;
        readonly CharacterController controller;
        readonly PlayerCollisionConfig collisionConfig;
        readonly PlayerMovementData movementData;
        readonly PlayerCollisionData collisionData;
        readonly RaycastHit[] obstacleHits = new RaycastHit[1];

        public ObstacleCheck(
            IMovementInput movementInput,
            CharacterController controller,
            PlayerCollisionConfig collisionConfig,
            PlayerMovementData movementData,
            PlayerCollisionData collisionData
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.collisionConfig = collisionConfig;
            this.movementData = movementData;
            this.collisionData = collisionData;
        }

        public void CheckObstacle()
        {
            if (!movementInput.HasMovement() || movementData.FinalMoveDirection.sqrMagnitude <= 0f)
                return;

            var hitCount = Physics.SphereCastNonAlloc(
                controller.transform.position + controller.center,
                collisionConfig.RayObstacleSphereRadius,
                movementData.FinalMoveDirection,
                obstacleHits,
                collisionConfig.RayObstacleLength,
                collisionConfig.ObstacleLayers
            );
            var hitWall = hitCount > 0;

            collisionData.HasObstructed = hitWall;
            collisionData.ObstructedNormal = hitWall ? obstacleHits[0].normal : Vector3.zero;
        }
    }
}
