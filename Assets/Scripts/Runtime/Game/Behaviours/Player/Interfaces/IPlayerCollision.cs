namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public interface IPlayerCollision
    {
        void GroundCheckHandler();
        void ObstacleCheckHandler();
        bool RoofCheckHandler();
    }
}
