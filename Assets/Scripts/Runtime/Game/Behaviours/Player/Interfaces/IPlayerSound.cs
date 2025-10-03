namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IPlayerSound
    {
        void UpdateFootsteps(float deltaTime);
        void UpdateLanding();
        void UpdateSwimming(float deltaTime);
        void UpdateClimbing(float deltaTime);
        void UpdateJumping();
        void UpdateDamaging();
    }
}
