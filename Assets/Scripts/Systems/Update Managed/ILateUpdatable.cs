namespace GameToolkit.Runtime.Systems.UpdateManaged
{
    public interface ILateUpdatable
    {
        void ProcessLateUpdate(float deltaTime);
    }
}
