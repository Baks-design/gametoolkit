using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class JumpHandler
    {
        readonly IMovementInput movementInput;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerSound soundController;
        float lastGroundedTime;
        float lastJumpPressTime;
        bool jumpWasPressed;

        public JumpHandler(
            IMovementInput movementInput,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig,
            PlayerSound soundController
        )
        {
            this.movementInput = movementInput;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
            this.soundController = soundController;
        }

        public void UpdateJumpBuffer()
        {
            // Coyote time: allow jumping shortly after leaving ground
            if (collisionData.OnGrounded)
                lastGroundedTime = Time.time;

            // Jump buffering: store jump input for a short time
            if (movementInput.JumpPressed())
            {
                lastJumpPressTime = Time.time;
                jumpWasPressed = true;
            }
        }

        public void HandleJump()
        {
            var hasCoyoteTime = Time.time - lastGroundedTime <= movementConfig.CoyoteTime;
            var hasJumpBuffer = Time.time - lastJumpPressTime <= movementConfig.JumpBufferTime;
            var canJump =
                (collisionData.OnGrounded || hasCoyoteTime)
                && hasJumpBuffer
                && !movementData.IsCrouching;

            if (!canJump)
                return;

            ExecuteJump();
            lastJumpPressTime = 0f; // Consume the buffered jump
            jumpWasPressed = false;
        }

        void ExecuteJump()
        {
            // Calculate proper jump velocity using physics: v = sqrt(2 * g * h)
            movementData.FinalMoveVelocity.y = Mathf.Sqrt(
                movementConfig.JumpHeight * -2f * Physics.gravity.y
            );
            collisionData.PreviouslyGrounded = true;
            collisionData.OnGrounded = false;

            soundController.PlayJumpSound();
        }
    }
}
