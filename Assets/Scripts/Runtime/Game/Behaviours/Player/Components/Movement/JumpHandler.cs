using GameToolkit.Runtime.Application.Input;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class JumpHandler
    {
        readonly PlayerCollisionData collisionData;
        readonly PlayerMovementData movementData;
        readonly PlayerMovementConfig movementConfig;
        readonly PlayerSoundController soundController;

        public JumpHandler(
            PlayerCollisionData collisionData,
            PlayerMovementData movementData,
            PlayerMovementConfig movementConfig,
            PlayerSoundController soundController
        )
        {
            this.collisionData = collisionData;
            this.movementData = movementData;
            this.movementConfig = movementConfig;
            this.soundController = soundController;
        }

        public void HandleJump()
        {
            if (!InputManager.JumpPressed || movementData.IsCrouching)
                return;

            movementData.FinalMoveVelocity.y = movementConfig.JumpHeight;
            //Logging.Log($"movementData.FinalMoveVelocity.y: {movementData.FinalMoveVelocity.y}");
            collisionData.PreviouslyGrounded = true;
            collisionData.OnGrounded = false;

            soundController.PlayJumpSound();
        }
    }
}
