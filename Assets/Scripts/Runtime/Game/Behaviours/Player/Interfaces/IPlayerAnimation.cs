namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IPlayerAnimation
    {
        void UpdateMoving();
        void UpdateJump();
        void UpdateCrouch();
        void UpdateSwimming();
        void UpdateClimbing();
        void TriggerAction(string actionName);
        void ResetActionTrigger(string actionName);
    }
}
