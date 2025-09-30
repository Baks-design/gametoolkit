using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class VelocityHandler
    {
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerMovementData movementData;
        readonly RunnningHandler runningHandler;
        readonly float walkRunSpeedDifference;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;

        public VelocityHandler(
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerMovementData movementData,
            RunnningHandler runningHandler
        )
        {
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.movementData = movementData;
            this.runningHandler = runningHandler;

            movementData.InAirTimer = 0f;
            walkRunSpeedDifference = movementConfig.RunSpeed - movementConfig.WalkSpeed;
        }

        public void CalculateSpeed()
        {
            movementData.CurrentSpeed =
                movementData.IsRunning && runningHandler.CanRun()
                    ? movementConfig.RunSpeed
                    : movementConfig.WalkSpeed;
            movementData.CurrentSpeed = movementData.IsCrouching
                ? movementConfig.CrouchSpeed
                : movementData.CurrentSpeed;
            movementData.CurrentSpeed = !InputManager.HasMovement ? 0f : movementData.CurrentSpeed;
            movementData.CurrentSpeed =
                InputManager.GetMovement.y == -1f
                    ? movementData.CurrentSpeed * movementConfig.MoveBackwardsSpeedPercent
                    : movementData.CurrentSpeed;
            movementData.CurrentSpeed =
                InputManager.GetMovement.x != 0f && InputManager.GetMovement.y == 0f
                    ? movementData.CurrentSpeed * movementConfig.MoveSideSpeedPercent
                    : movementData.CurrentSpeed;

            //Logging.Log($"CurrentSpeed: {currentSpeed}");
        }

        public void SmoothSpeed(float deltaTime)
        {
            smoothCurrentSpeed = Mathf.Lerp(
                smoothCurrentSpeed,
                movementData.CurrentSpeed,
                deltaTime * movementConfig.SmoothVelocitySpeed
            );

            //Logging.Log($"SmoothCurrentSpeed: {smoothCurrentSpeed}");

            if (movementData.IsRunning && runningHandler.CanRun())
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
            movementData.InAirTimer = 0f;
            movementData.FinalMoveVelocity.y = -movementConfig.StickToGroundForce;
        }

        public void ApplyGravityOnAirborne(float deltaTime)
        {
            movementData.InAirTimer += deltaTime;
            movementData.FinalMoveVelocity +=
                deltaTime * movementConfig.GravityMultiplier * Physics.gravity;
        }

        public void CalculateFinalGroundedAcceleration()
        {
            var targetVelocity = finalSmoothCurrentSpeed * movementData.SmoothFinalMoveDir;

            movementData.FinalMoveVelocity.x = targetVelocity.x;
            movementData.FinalMoveVelocity.y += targetVelocity.y;
            movementData.FinalMoveVelocity.z = targetVelocity.z;

            //Logging.Log($"movementData.FinalMoveVelocity: {movementData.FinalMoveVelocity}");
        }

        public void CalculateFinalAirborneAcceleration()
        {
            var targetVelocity = finalSmoothCurrentSpeed * movementData.SmoothFinalMoveDir;

            movementData.FinalMoveVelocity.x = targetVelocity.x;
            movementData.FinalMoveVelocity.z = targetVelocity.z;

            //Logging.Log($"movementData.FinalMoveVelocity: {movementData.FinalMoveVelocity}");
        }

        public void ApplyMove(float deltaTime)
        {
            controller.Move(movementData.FinalMoveVelocity * deltaTime);

            movementData.VerticalVelocity = movementData.FinalMoveVelocity.y;

            movementData.IsMoving = controller.velocity.magnitude > 0.1f;
            //Logging.Log($"movementData.IsMoving: {movementData.IsMoving}");

            movementData.IsWalking =
                Mathf.Abs(controller.velocity.x) > 0.1f || Mathf.Abs(controller.velocity.z) > 0.1f;
            //Logging.Log($"movementData.IsWalking: {movementData.IsWalking}");

            movementData.CurrentVelocity = controller.velocity;
            //Logging.Log($"movementData.CurrentVelocity: {movementData.CurrentVelocity}");
        }
    }
}
