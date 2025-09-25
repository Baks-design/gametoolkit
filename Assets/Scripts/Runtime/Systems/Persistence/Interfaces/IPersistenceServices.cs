namespace GameToolkit.Runtime.Systems.Persistence
{
    public interface IPersistenceServices
    {
        public void NewGame();
        public void SaveGame();
        public void LoadGame(string gameName);
        public void ReloadGame();
        public void DeleteGame(string gameName);
    }
}
