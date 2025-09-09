using System.Collections.Generic;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameToolkit.Runtime.Systems.Persistence
{
    public class PersistenceManager : MonoBehaviour, IPersistenceServices
    {
        IDataService dataService;

        [field: SerializeField]
        public GameData GameData { get; private set; }

        void Awake()
        {
            Setup();
            InitializeClasses();
        }

        void Setup()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IPersistenceServices>(this);
        }

        void InitializeClasses() => dataService = new FileDataService(new JsonSerializer());

#pragma warning disable UDR0005 // Domain Reload Analyzer
        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
#pragma warning restore UDR0005 // Domain Reload Analyzer

        void Start() => NewGame();

        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu")
                return;

            Bind<PlayerDataHandler, PlayerData>(GameData.playerData);
        }

        void Bind<T, TData>(TData data)
            where T : MonoBehaviour, IBind<TData>
            where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            T entity = null;
            if (entities.Length > 0)
                entity = entities[0];
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas)
            where T : MonoBehaviour, IBind<TData>
            where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var entity in entities)
            {
                TData data = default;
                foreach (var d in datas)
                {
                    if (d.Id == entity.Id)
                    {
                        data = d;
                        break;
                    }
                }
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            GameData = new GameData { Name = "Game", CurrentLevelName = "Demo" };
            if (GameData != null)
                SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void SaveGame() => dataService.Save(GameData);

        public void LoadGame(string gameName)
        {
            GameData = dataService.Load(gameName);

            if (string.IsNullOrWhiteSpace(GameData.CurrentLevelName))
                GameData.CurrentLevelName = "Demo";

            SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(GameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}
