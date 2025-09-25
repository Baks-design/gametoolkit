namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface ILateUpdatable
    {
        void ProcessLateUpdate(float deltaTime);
    }
}
