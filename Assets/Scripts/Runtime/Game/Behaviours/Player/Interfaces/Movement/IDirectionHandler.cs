namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IDirectionHandler
    {
        void SmoothInput(float deltaTime);
        void CalculateMovementGroundedDirection();
        void CalculateMovementAirborneDirection();
        void SmoothDirection(float deltaTime);
    }
}
