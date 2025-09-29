using GameToolkit.Runtime.Utils.Helpers;

namespace GameToolkit.Runtime.Application.Persistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}
