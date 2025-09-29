namespace GameToolkit.Runtime.Game.Systems.Update
{
    public interface ILateUpdateServices
    {
        void Register(ILateUpdatable obj);
        void Unregister(ILateUpdatable obj);
    }
}
