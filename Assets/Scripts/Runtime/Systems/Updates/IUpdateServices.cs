namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IUpdateServices
    {
        void Register(IManagedObject updatable);
        void Unregister(IManagedObject updatable);
    }
}
