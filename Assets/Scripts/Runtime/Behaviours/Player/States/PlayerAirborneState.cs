using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAirborneState : IState
    {
        readonly PlayerMovement playerMovement;

        public PlayerAirborneState(PlayerMovement playerMovement) =>
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
            playerMovement.ApplyGravityOnAirborne(deltaTime);
            playerMovement.ApplyMove(deltaTime);
        }

        public void LateUpdate(float deltaTime) { }

        public void OnExit() { }
    }
}
