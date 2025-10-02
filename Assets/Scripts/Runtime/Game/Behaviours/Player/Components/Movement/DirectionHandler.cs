using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class DirectionHandler
    {
        readonly IMovementInput movementInput;
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;

        public DirectionHandler(
            IMovementInput movementInput,
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;
        }

        public void SmoothInput(float deltaTime) =>
            movementData.SmoothInputVector = Vector2.Lerp(
                movementData.SmoothInputVector,
                movementInput.GetMovement().normalized,
                deltaTime * movementConfig.SmoothInputSpeed
            );

        public void CalculateMovementDirectionOnGrounded()
        {
            // Cache the smooth input vector to avoid multiple property accesses
            var smoothInput = movementData.SmoothInputVector;
            // Calculate world space direction
            var desiredDir = CalculateWorldDirection(smoothInput);
            // Apply slope handling if grounded
            movementData.FinalMoveDirection = FlattenVectorOnSlopes(desiredDir);
        }

        public void CalculateMovementDirectionOnAir()
        {
            // Cache the smooth input vector to avoid multiple property accesses
            var smoothInput = movementData.SmoothInputVector;
            // Calculate world space direction
            var desiredDir = CalculateWorldDirection(smoothInput);
            // Apply slope handling if grounded
            movementData.FinalMoveDirection = desiredDir;
        }

        public void SmoothDirection(float deltaTime) =>
            movementData.SmoothFinalMoveDir = Vector3.Lerp(
                movementData.SmoothFinalMoveDir,
                movementData.FinalMoveDirection,
                deltaTime * movementConfig.SmoothFinalDirectionSpeed
            );

        Vector3 CalculateWorldDirection(Vector2 input)
        {
            var zDir = controller.transform.forward * input.y;
            var xDir = controller.transform.right * input.x;

            var desiredDir = xDir + zDir;
            desiredDir.y = 0f;

            return desiredDir.normalized; // Always normalize for consistent speed
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vector) =>
            Vector3.ProjectOnPlane(vector, collisionData.GroundedNormal).normalized;
    }
}
