using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class VelocityHandler
    {
        readonly CharacterController controller;
        readonly Transform yawTransform;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly float walkRunSpeedDifference;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;
        float currentSpeed;
        float targetVerticalVelocity;

        public VelocityHandler(
            CharacterController controller,
            Transform yawTransform,
            PlayerMovementConfig movementConfig,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData
        )
        {
            this.controller = controller;
            this.yawTransform = yawTransform;
            this.movementConfig = movementConfig;
            this.collisionData = collisionData;
            this.movementData = movementData;

            movementData.InAirTimer = 0f;
            targetVerticalVelocity = 0f;
            walkRunSpeedDifference = movementConfig.RunSpeed - movementConfig.WalkSpeed;
        }

        public void CalculateSpeed()
        {
            currentSpeed =
                movementData.IsRunning && CanRun()
                    ? movementConfig.RunSpeed
                    : movementConfig.WalkSpeed;
            currentSpeed = movementData.IsCrouching ? movementConfig.CrouchSpeed : currentSpeed;
            currentSpeed = !InputManager.HasMovement ? 0f : currentSpeed;
            currentSpeed =
                InputManager.GetMovement.y == -1f
                    ? currentSpeed * movementConfig.MoveBackwardsSpeedPercent
                    : currentSpeed;
            currentSpeed =
                InputManager.GetMovement.x != 0f && InputManager.GetMovement.y == 0f
                    ? currentSpeed * movementConfig.MoveSideSpeedPercent
                    : currentSpeed;

            //Logging.Log($"CurrentSpeed: {currentSpeed}");
        }

        public void SmoothSpeed(float deltaTime)
        {
            smoothCurrentSpeed = Mathf.Lerp(
                smoothCurrentSpeed,
                currentSpeed,
                deltaTime * movementConfig.SmoothVelocitySpeed
            );

            //Logging.Log($"SmoothCurrentSpeed: {smoothCurrentSpeed}");

            if (movementData.IsRunning && CanRun())
            {
                var walkRunPercent = Mathf.InverseLerp(
                    movementConfig.WalkSpeed,
                    movementConfig.RunSpeed,
                    smoothCurrentSpeed
                );
                finalSmoothCurrentSpeed =
                    movementConfig.RunTransitionCurve.Evaluate(walkRunPercent)
                        * walkRunSpeedDifference
                    + movementConfig.WalkSpeed;
            }
            else
                finalSmoothCurrentSpeed = smoothCurrentSpeed;

            //Logging.Log($"FinalSmoothCurrentSpeed: {finalSmoothCurrentSpeed}");
        }

        public void ApplyGravityOnGrounded()
        {
            if (!controller.isGrounded)
                return;

            movementData.InAirTimer = 0f;
            targetVerticalVelocity = -movementConfig.StickToGroundForce;
        }

        public void ApplyGravityOnAirborne(float deltaTime)
        {
            if (controller.isGrounded)
                return;

            movementData.InAirTimer += deltaTime;
            targetVerticalVelocity +=
                deltaTime * movementConfig.GravityMultiplier * Physics.gravity.y;
        }

        float CalculateVerticalAcceleration(float deltaTime) =>
            movementConfig.VerticalPID.Calculate(
                targetVerticalVelocity,
                movementData.FinalMoveVelocity.y,
                deltaTime
            );

        Vector2 CalculateHorizontalAcceleration(float deltaTime)
        {
            var targetHorizontalVelocity = finalSmoothCurrentSpeed * smoothFinalMoveDir;

            var horizontalAcceleration = new Vector3(
                movementConfig.HorizontalPID.Calculate(
                    targetHorizontalVelocity.x,
                    movementData.FinalMoveVelocity.x,
                    deltaTime
                ),
                0f,
                movementConfig.HorizontalPID.Calculate(
                    targetHorizontalVelocity.z,
                    movementData.FinalMoveVelocity.z,
                    deltaTime
                )
            );

            return horizontalAcceleration;
        }

        public void CalculateFinalAcceleration(float deltaTime)
        {
            var horizontalAcceleration = CalculateHorizontalAcceleration(deltaTime);
            var verticalAcceleration = CalculateVerticalAcceleration(deltaTime);

            var totalAcceleration = new Vector3(
                horizontalAcceleration.x,
                verticalAcceleration,
                horizontalAcceleration.y
            );

            movementData.FinalMoveVelocity += totalAcceleration * deltaTime;

            //Logging.Log($"movementData.FinalMoveVelocity: {movementData.FinalMoveVelocity}");
        }

        public void ApplyMove(float deltaTime)
        {
            controller.Move(movementData.FinalMoveVelocity * deltaTime);

            movementData.IsMoving = Mathf.Abs(controller.velocity.sqrMagnitude) > 0.1f;
            //Logging.Log($"movementData.IsMoving : {movementData.IsMoving }");
            movementData.IsWalking =
                Mathf.Abs(controller.velocity.x) > 0.1f || Mathf.Abs(controller.velocity.z) > 0.1f;
            //Logging.Log($"movementData.IsWalking: {movementData.IsWalking}");
            movementData.CurrentVelocity = controller.velocity;
            //Logging.Log($"movementData.CurrentVelocity: {movementData.CurrentVelocity}");
        }
    }
}
