using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using ZLinq;

namespace GameToolkit.Runtime.Application.Persistence
{
    public class PersistenceManager : MonoBehaviour, IPersistenceServices
    {
        [SerializeField, ReadOnly]
        GameData gameData;
        IDataService dataService;

        public GameData GameData => gameData;

        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            dataService = new FileDataService(new JsonSerializer());
        }

        public void Bind<T, TData>(TData data)
            where T : MonoBehaviour, IBind<TData>
            where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None)
                .AsValueEnumerable()
                .FirstOrDefault();
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        public void Bind<T, TData>(List<TData> datas)
            where T : MonoBehaviour, IBind<TData>
            where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var entity in entities)
            {
                var data = datas.AsValueEnumerable().FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame() =>
            gameData = new GameData { Name = "Game", CurrentLevelName = "Demo" };

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);
            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName))
                gameData.CurrentLevelName = "Demo";
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}
