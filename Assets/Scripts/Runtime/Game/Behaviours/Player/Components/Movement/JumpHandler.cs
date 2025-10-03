using GameToolkit.Runtime.Application.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class JumpHandler
    {
        readonly CharacterController controller;
        readonly IMovementInput movementInput;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;
        float lastGroundedTime;
        float lastJumpPressTime;

        public JumpHandler(
            CharacterController controller,
            IMovementInput movementInput,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.controller = controller;
            this.movementInput = movementInput;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
        }

        public void HandleJump(float time)
        {
            ExecuteJump(time);
            ResetJump();
        }

        void ExecuteJump(float time)
        {
            var hasCoyoteTime = time - lastGroundedTime <= movementConfig.CoyoteTime;
            var hasJumpBuffer = time - lastJumpPressTime <= movementConfig.JumpBufferTime;
            var canJump =
                (controller.isGrounded || hasCoyoteTime)
                && hasJumpBuffer
                && !movementData.IsCrouching;
            if (!canJump)
                return;

            movementData.FinalMoveVelocity.y = movementConfig.JumpHeight;
            collisionData.PreviouslyGrounded = true;
            movementData.IsJumping = true;
            lastJumpPressTime = 0f;
        }

        void ResetJump()
        {
            if (movementData.FinalMoveVelocity.y <= 0.1f || collisionData.OnGrounded)
                movementData.IsJumping = false;
        }

        public void UpdateJumpBuffer(float time)
        {
            lastGroundedTime = time;
            if (movementInput.JumpPressed())
                lastJumpPressTime = time;
        }
    }
}
