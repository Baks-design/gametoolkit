using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class DirectionHandler
    {
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;

        public DirectionHandler(
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData
        )
        {
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;
        }

        public void SmoothInput(float deltaTime)
        {
            //Logging.Log($"FinalMoveDirection: {InputManager.GetMovement.normalized}");

            movementData.SmoothInputVector = Vector2.Lerp(
                movementData.SmoothInputVector,
                InputManager.GetMovement.normalized,
                deltaTime * movementConfig.SmoothInputSpeed
            );

            //Logging.Log($"SmoothInputVector: {movementData.SmoothInputVector}");
        }

        public void CalculateMovementGroundedDirection()
        {
            var zDir = controller.transform.forward * movementData.SmoothInputVector.y;
            var xDir = controller.transform.right * movementData.SmoothInputVector.x;

            var desiredDir = xDir + zDir;
            desiredDir.y = 0f;

            //Logging.Log($"desiredDir: {desiredDir}");

            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            movementData.FinalMoveDirection = flattenDir;

            //Logging.Log($"FinalMoveDirection: {movementData.FinalMoveDirection}");
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat) =>
            Vector3.ProjectOnPlane(vectorToFlat, collisionData.GroundedNormal);

        public void CalculateMovementAirborneDirection()
        {
            var zDir = controller.transform.forward * movementData.SmoothInputVector.y;
            var xDir = controller.transform.right * movementData.SmoothInputVector.x;

            var desiredDir = xDir + zDir;
            desiredDir.y = 0f;

            //Logging.Log($"desiredDir: {desiredDir}");

            movementData.FinalMoveDirection = desiredDir;

            //Logging.Log($"FinalMoveDirection: {movementData.FinalMoveDirection}");
        }

        public void SmoothDirection(float deltaTime)
        {
            movementData.SmoothFinalMoveDir = Vector3.Lerp(
                movementData.SmoothFinalMoveDir,
                movementData.FinalMoveDirection,
                deltaTime * movementConfig.SmoothFinalDirectionSpeed
            );

            //Logging.Log($"SmoothFinalMoveDir: {movementData.SmoothFinalMoveDir}");
        }
    }
}
