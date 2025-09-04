using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerGroundedState : IState
    {
        readonly PlayerMovement playerMovement;

        public PlayerGroundedState(PlayerMovement playerMovement) =>
            this.playerMovement = playerMovement;

        public void OnEnter() { }

        public void FixedUpdate(float deltaTime) { }

        public void Update(float deltaTime)
        {
            playerMovement.RotateTowardsCamera(deltaTime);
            playerMovement.SmoothInput(deltaTime);
            playerMovement.SmoothSpeed(deltaTime);
            playerMovement.SmoothDirection(deltaTime);
            playerMovement.CalculateMovementDirection();
            playerMovement.CalculateSpeed();
            playerMovement.CalculateFinalMovement();
            playerMovement.ApplyGravityOnGrounded();
            playerMovement.ApplyMove(deltaTime);
        }

        public void LateUpdate(float deltaTime) { }

        public void OnExit() { }
    }
}
