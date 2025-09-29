namespace GameToolkit.Runtime.Application.Persistence
{
    public interface IPersistenceServices
    {
        void NewGame();
        void SaveGame();
        void LoadGame(string gameName);
        void ReloadGame();
        void DeleteGame(string gameName);
    }
}
