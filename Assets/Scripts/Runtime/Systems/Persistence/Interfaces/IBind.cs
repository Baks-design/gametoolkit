using GameToolkit.Runtime.Utils.Helpers;

namespace GameToolkit.Runtime.Systems.Persistence
{
    public interface IBind<TData>
        where TData : ISaveable
    {
        SerializableGuid Id { get; set; }

        void Bind(TData data);
    }
}
