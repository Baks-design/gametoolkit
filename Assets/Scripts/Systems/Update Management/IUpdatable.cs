namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IUpdatable
    {
        virtual void ProcessUpdate(float deltaTime) { }
    }
}
