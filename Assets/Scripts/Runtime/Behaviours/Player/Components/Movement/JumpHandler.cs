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

        public void HandleJump()
        {
            if (!InputManager.JumpPressed || movementData.IsCrouching)
                return;

            movementData.FinalMoveVelocity.y = movementConfig.JumpHeight;
            //Logging.Log($"movementData.FinalMoveVelocity.y: {movementData.FinalMoveVelocity.y}");

            collisionData.PreviouslyGrounded = true;
            collisionData.OnGrounded = false;
        }
    }
}
