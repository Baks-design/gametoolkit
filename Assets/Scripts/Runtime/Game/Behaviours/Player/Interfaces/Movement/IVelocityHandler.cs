namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IVelocityHandler
    {
        void CalculateSpeed();
        void SmoothSpeed(float deltaTime);
        void ApplyGravityOnGrounded();
        void ApplyGravityOnAirborne(float deltaTime);
        void CalculateFinalAcceleration();
        void ApplyMove(float deltaTime);
    }
}
