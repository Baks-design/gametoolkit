namespace GameToolkit.Runtime.Systems.UpdateManagement
{
    public interface IFixedUpdateServices
    {
        void Register(IFixedUpdatable obj);
        void Unregister(IFixedUpdatable obj);
    }
}
