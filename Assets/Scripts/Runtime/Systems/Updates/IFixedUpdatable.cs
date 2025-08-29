namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IFixedUpdatable
    {
        virtual void ProcessFixedUpdate(float deltaTime) { }
    }
}
