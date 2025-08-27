namespace GameToolkit.Runtime.Systems.Persistence
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
