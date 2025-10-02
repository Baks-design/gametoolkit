namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface ICameraHandler
    {
        void RotateTowardsCamera(float deltaTime);
        void HandleHeadBob(float deltaTime);
        void HandleCameraSway(float deltaTime);
        void HandleRunFOV(float deltaTime);
    }
}
