namespace GameToolkit.Runtime.Systems.Culling
{
    public interface ICullingServices
    {
        void Register(CullingTarget t);
        void Deregister(CullingTarget t);
    }
}
