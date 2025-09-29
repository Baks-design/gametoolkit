namespace GameToolkit.Runtime.Application.Persistence
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}
