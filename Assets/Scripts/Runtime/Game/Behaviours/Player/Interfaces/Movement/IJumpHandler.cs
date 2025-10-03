namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IJumpingHandler
    {
        void HandleJumping(float time);
        void UpdateJumpBuffer(float time);
    }
}
