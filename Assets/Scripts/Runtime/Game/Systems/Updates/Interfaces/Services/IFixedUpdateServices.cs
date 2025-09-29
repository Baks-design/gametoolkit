namespace GameToolkit.Runtime.Game.Systems.Update
{
    public interface IFixedUpdateServices
    {
        void Register(IFixedUpdatable obj);
        void Unregister(IFixedUpdatable obj);
    }
}
