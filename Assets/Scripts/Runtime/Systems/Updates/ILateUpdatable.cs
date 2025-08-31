namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface ILateUpdatable
    {
        public virtual void ProcessLateUpdate(float deltaTime) { }
    }
}
