using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class DirectionHandler
    {
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        Vector3 smoothFinalMoveDir;
        Vector2 smoothInputVector;

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
            smoothInputVector = Vector2.Lerp(
                smoothInputVector,
                InputManager.GetMovement,
                deltaTime * movementConfig.SmoothInputSpeed
            );

            //Logging.Log($"SmoothInputVector: {smoothInputVector}");
        }

        public void CalculateMovementDirection()
        {
            var vDir = controller.transform.forward * smoothInputVector.y;
            var hDir = controller.transform.right * smoothInputVector.x;
            var desiredDir = vDir + hDir;
            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            movementData.FinalMoveDirection = flattenDir;

            //Logging.Log($"FinalMoveDirection: {movementData.FinalMoveDirection}");
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
        {
            if (!collisionData.OnGrounded)
                return vectorToFlat;
            vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, collisionData.GroundedNormal);
            return vectorToFlat;
        }

        public void SmoothDirection(float deltaTime)
        {
            smoothFinalMoveDir = Vector3.Lerp(
                smoothFinalMoveDir,
                movementData.FinalMoveDirection,
                deltaTime * movementConfig.SmoothFinalDirectionSpeed
            );

            //Logging.Log($"SmoothFinalMoveDir: {smoothFinalMoveDir}");
        }
    }
}
