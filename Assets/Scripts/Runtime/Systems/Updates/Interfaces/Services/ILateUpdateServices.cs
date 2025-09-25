namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface ILateUpdateServices
    {
        void Register(ILateUpdatable obj);
        void Unregister(ILateUpdatable obj);
    }
}
