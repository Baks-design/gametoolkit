using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
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
        float currentSpeed;

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
            currentSpeed = movementConfig.WalkSpeed;

            if (InputManager.HasMovement)
            {
                if (movementData.IsCrouching)
                    currentSpeed = movementConfig.CrouchSpeed;
                else if (movementData.IsRunning && runningHandler.CanRun())
                    currentSpeed = movementConfig.RunSpeed;

                // Apply direction modifiers
                if (InputManager.GetMovement.y == -1f)
                    currentSpeed *= movementConfig.MoveBackwardsSpeedPercent;
                else if (InputManager.GetMovement.x != 0f && InputManager.GetMovement.y == 0f)
                    currentSpeed *= movementConfig.MoveSideSpeedPercent;
            }
            else
                currentSpeed = 0f;

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
            if (!controller.isGrounded)
                return;

            movementData.InAirTimer = 0f;
            movementData.FinalMoveVelocity.y = -movementConfig.StickToGroundForce;
        }

        public void ApplyGravityOnAirborne(float deltaTime)
        {
            if (controller.isGrounded)
                return;

            movementData.InAirTimer += deltaTime;
            movementData.FinalMoveVelocity +=
                deltaTime * movementConfig.GravityMultiplier * Physics.gravity;
        }

        public void CalculateFinalAcceleration()
        {
            var targetVelocity = finalSmoothCurrentSpeed * movementData.SmoothFinalMoveDir;

            movementData.FinalMoveVelocity.x = targetVelocity.x;
            movementData.FinalMoveVelocity.z = targetVelocity.z;
            if (controller.isGrounded)
                movementData.FinalMoveVelocity.y += targetVelocity.y;

            //Logging.Log($"movementData.FinalMoveVelocity: {movementData.FinalMoveVelocity}");
        }

        public void ApplyMove(float deltaTime)
        {
            controller.Move(movementData.FinalMoveVelocity * deltaTime);

            movementData.IsMoving = controller.velocity.sqrMagnitude > 0.1f;
            //Logging.Log($"movementData.IsMoving: {movementData.IsMoving}");
            movementData.IsWalking =
                Mathf.Abs(controller.velocity.x) > 0.1f || Mathf.Abs(controller.velocity.z) > 0.1f;
            //Logging.Log($"movementData.IsWalking: {movementData.IsWalking}");
            movementData.CurrentVelocity = controller.velocity;
            //Logging.Log($"movementData.CurrentVelocity: {movementData.CurrentVelocity}");
        }
    }
}
