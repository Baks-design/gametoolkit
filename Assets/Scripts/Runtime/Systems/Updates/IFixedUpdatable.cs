namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IFixedUpdatable
    {
        public virtual void ProcessFixedUpdate(float deltaTime) { }
    }
}
