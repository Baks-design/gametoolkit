using System;

namespace GameToolkit.Runtime.Application.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
    }
}
