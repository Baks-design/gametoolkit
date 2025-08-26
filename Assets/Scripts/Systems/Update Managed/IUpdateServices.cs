namespace GameToolkit.Runtime.Systems.UpdateManaged
{
    public interface IUpdateServices
    {
        void Register(IManagedObject updatable);
        void Unregister(IManagedObject updatable);
    }
}
