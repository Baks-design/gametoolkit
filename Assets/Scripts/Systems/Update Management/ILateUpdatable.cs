namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface ILateUpdatable
    {
        virtual void ProcessLateUpdate(float deltaTime) { }
    }
}
