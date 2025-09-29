namespace GameToolkit.Runtime.Game.Systems.Update
{
    public interface ILateUpdatable
    {
        void ProcessLateUpdate(float deltaTime);
    }
}
