namespace GameToolkit.Runtime.Game.Systems.Update
{
    public interface IUpdateServices
    {
        void Register(IUpdatable obj);
        void Unregister(IUpdatable obj);
    }
}
