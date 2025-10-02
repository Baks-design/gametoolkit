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
        float lastGroundedTime;
        float lastJumpPressTime;

        public JumpHandler(
            IMovementInput movementInput,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.movementInput = movementInput;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
        }

        public void UpdateJumpBuffer(float time)
        {
            lastGroundedTime = time;
            if (movementInput.JumpPressed())
                lastJumpPressTime = time;
        }

        public void HandleJump(float time)
        {
            var hasCoyoteTime = time - lastGroundedTime <= movementConfig.CoyoteTime;
            var hasJumpBuffer = time - lastJumpPressTime <= movementConfig.JumpBufferTime;
            var canJump =
                (collisionData.OnGrounded || hasCoyoteTime)
                && hasJumpBuffer
                && !movementData.IsCrouching;
            if (!canJump)
                return;

            ExecuteJump();
            lastJumpPressTime = 0f;
        }

        void ExecuteJump()
        {
            movementData.FinalMoveVelocity.y = Mathf.Sqrt(
                movementConfig.JumpHeight * -2f * Physics.gravity.y
            );

            collisionData.PreviouslyGrounded = true;
            movementData.IsJumping = true;
        }

        public void ResetJump()
        {
            if (movementData.FinalMoveVelocity.y <= 0.1f || collisionData.OnGrounded)
                movementData.IsJumping = false;
        }
    }
}
