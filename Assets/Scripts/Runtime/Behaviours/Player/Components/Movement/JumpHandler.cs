using GameToolkit.Runtime.Systems.Input;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class JumpHandler
    {
        readonly CharacterController controller;
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;

        public JumpHandler(
            CharacterController controller,
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig
        )
        {
            this.controller = controller;
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
        }

        public void HandleJump(float deltaTime)
        {
            if (
                !InputManager.JumpPressed
                || !controller.isGrounded
                || movementData.IsCrouching
                || !movementData.IsRunning
            )
                return;

            movementData.FinalMoveVelocity.y = Mathf.Sqrt(
                movementConfig.JumpHeight * deltaTime * -2f
            );
            collisionData.PreviouslyGrounded = true;
            collisionData.OnGrounded = false;

            //Logging.Log($"movementData.FinalMoveVelocity.y: {movementData.FinalMoveVelocity.y}");
        }
    }
}
