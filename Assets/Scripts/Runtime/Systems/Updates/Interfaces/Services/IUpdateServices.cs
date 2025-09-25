namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IUpdateServices
    {
        void Register(IUpdatable obj);
        void Unregister(IUpdatable obj);
    }
}
