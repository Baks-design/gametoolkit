namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IUpdatable
    {
        public virtual void ProcessUpdate(float deltaTime) { }
    }
}
