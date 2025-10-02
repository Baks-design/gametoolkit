using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class VelocityHandler
    {
        readonly IMovementInput movementInput;
        readonly CharacterController controller;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerMovementData movementData;
        readonly RunnningHandler runningHandler;
        float smoothCurrentSpeed;
        float finalSmoothCurrentSpeed;

        public VelocityHandler(
            IMovementInput movementInput,
            CharacterController controller,
            PlayerMovementConfig movementConfig,
            PlayerMovementData movementData,
            RunnningHandler runningHandler
        )
        {
            this.movementInput = movementInput;
            this.controller = controller;
            this.movementConfig = movementConfig;
            this.movementData = movementData;
            this.runningHandler = runningHandler;

            movementData.InAirTimer = 0f;
        }

        public void CalculateSpeed()
        {
            // Start with base speed based on movement state
            var baseSpeed = CalculateBaseSpeed();
            // Apply movement direction modifiers
            movementData.CurrentSpeed = ApplyDirectionModifiers(baseSpeed);

            // Handle no movement case
            if (!movementInput.HasMovement())
                movementData.CurrentSpeed = 0f;
        }

        public void SmoothSpeed(float deltaTime)
        {
            smoothCurrentSpeed = Mathf.Lerp(
                smoothCurrentSpeed,
                movementData.CurrentSpeed,
                deltaTime * movementConfig.SmoothVelocitySpeed
            );

            finalSmoothCurrentSpeed =
                movementData.IsRunning && runningHandler.CanRun()
                    ? CalculateRunTransitionSpeed()
                    : smoothCurrentSpeed;
        }

        public void ApplyGravityOnGrounded()
        {
            movementData.InAirTimer = 0f;
            movementData.FinalMoveVelocity.y = -movementConfig.StickToGroundForce;
        }

        public void ApplyGravityOnAir(float deltaTime)
        {
            movementData.InAirTimer += deltaTime;
            movementData.FinalMoveVelocity +=
                deltaTime * movementConfig.GravityMultiplier * Physics.gravity;
        }

        public void CalculateFinalAccelerationOnGrounded()
        {
            var targetVelocity = finalSmoothCurrentSpeed * movementData.SmoothFinalMoveDir;
            movementData.FinalMoveVelocity = new Vector3(
                targetVelocity.x,
                movementData.FinalMoveVelocity.y + targetVelocity.y, // Maintain gravity with Y movement
                targetVelocity.z
            );
        }

        public void CalculateFinalAccelerationOnAir()
        {
            var targetVelocity = finalSmoothCurrentSpeed * movementData.SmoothFinalMoveDir;
            movementData.FinalMoveVelocity = new Vector3(
                targetVelocity.x,
                movementData.FinalMoveVelocity.y, // Preserve existing Y velocity (gravity)
                targetVelocity.z
            );
        }

        public void ApplyMove(float deltaTime)
        {
            controller.Move(movementData.FinalMoveVelocity * deltaTime);
            UpdateMovementState();
        }

        float CalculateBaseSpeed()
        {
            if (movementData.IsCrouching)
                return movementConfig.CrouchSpeed;

            if (movementData.IsRunning && runningHandler.CanRun())
                return movementConfig.RunSpeed;

            return movementConfig.WalkSpeed;
        }

        float ApplyDirectionModifiers(float baseSpeed)
        {
            var input = movementInput.GetMovement();

            // Moving backwards
            if (input.y < -0.1f)
                return baseSpeed * movementConfig.MoveBackwardsSpeedPercent;

            // Pure sideways movement
            if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f)
                return baseSpeed * movementConfig.MoveSideSpeedPercent;

            return baseSpeed;
        }

        float CalculateRunTransitionSpeed()
        {
            var walkRunPercent = Mathf.InverseLerp(
                movementConfig.WalkSpeed,
                movementConfig.RunSpeed,
                smoothCurrentSpeed
            );

            return movementConfig.RunTransitionCurve.Evaluate(walkRunPercent)
                    * (movementConfig.RunSpeed - movementConfig.WalkSpeed)
                + movementConfig.WalkSpeed;
        }

        void UpdateMovementState()
        {
            var velocity = controller.velocity;
            var horizontalSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;

            movementData.VerticalVelocity = velocity.y;
            movementData.IsMoving = horizontalSpeed > 0.1f;
            movementData.IsWalking = movementData.IsMoving && !movementData.IsRunning;
            movementData.CurrentVelocity = velocity;
        }
    }
}
