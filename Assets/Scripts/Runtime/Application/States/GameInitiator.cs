using Alchemy.Inspector;
using GameToolkit.Runtime.Application.Persistence;
using GameToolkit.Runtime.Application.Scenes;
using GameToolkit.Runtime.Game.Systems.Culling;
using GameToolkit.Runtime.Game.Systems.Sound;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.UI;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameToolkit.Runtime.Application.States
{
    public class GameInitiator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, AssetsOnly, Required]
        CinemachineBrain cinemachineBrain;

        [SerializeField, AssetsOnly, Required]
        EventSystem eventSystem;

        [SerializeField, AssetsOnly, Required]
        LoadingScreen loadingScreen;

        [SerializeField, AssetsOnly, Required]
        AssetReferenceGameObject playerReference;
        GameObject player;

        [Header("Managers")]
        [SerializeField, AssetsOnly, Required]
        SceneLoaderManager sceneLoaderManager;

        [SerializeField, AssetsOnly, Required]
        PersistenceManager persistenceManager;

        [SerializeField, AssetsOnly, Required]
        StateManager stateManager;

        [SerializeField, AssetsOnly, Required]
        UpdateManager updateManager;

        [SerializeField, AssetsOnly, Required]
        FixedUpdateManager fixedUpdateManager;

        [SerializeField, AssetsOnly, Required]
        LateUpdateManager lateUpdateManager;

        [SerializeField, AssetsOnly, Required]
        SoundManager soundManager;

        [SerializeField, AssetsOnly, Required]
        MusicManager musicManager;

        [SerializeField, AssetsOnly, Required]
        CullingManager cullingManager;

        async void Awake()
        {
            BindObjects();

            ServicesRegistration();

            using var loadingScreenDisposable = new ShowLoadingScreenDisposable(loadingScreen);
            loadingScreenDisposable.SetLoadingBarPercent(0f);
            await InitializeObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.33f);
            await CreateObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.66f);
            PrepareGame();
            loadingScreenDisposable.SetLoadingBarPercent(1f);

            BeginGame();
        }

        void BindObjects()
        {
            cinemachineBrain = Instantiate(cinemachineBrain);
            eventSystem = Instantiate(eventSystem);
            loadingScreen = Instantiate(loadingScreen);
            sceneLoaderManager = Instantiate(sceneLoaderManager);
            persistenceManager = Instantiate(persistenceManager);
            stateManager = Instantiate(stateManager);
            updateManager = Instantiate(updateManager);
            fixedUpdateManager = Instantiate(fixedUpdateManager);
            lateUpdateManager = Instantiate(lateUpdateManager);
            soundManager = Instantiate(soundManager);
            musicManager = Instantiate(musicManager);
            cullingManager = Instantiate(cullingManager);
        }

        void ServicesRegistration()
        {
            ServiceLocator.Global.Register<ISceneLoaderServices>(sceneLoaderManager);
            ServiceLocator.Global.Register<IPersistenceServices>(persistenceManager);
            ServiceLocator.Global.Register<IStateServices>(stateManager);
            ServiceLocator.Global.Register<ISoundServices>(soundManager);
            ServiceLocator.Global.Register<IMusicServices>(musicManager);
            ServiceLocator.Global.Register<ICullingServices>(cullingManager);
            ServiceLocator.Global.Register<IUpdateServices>(updateManager);
            ServiceLocator.Global.Register<IFixedUpdateServices>(fixedUpdateManager);
            ServiceLocator.Global.Register<ILateUpdateServices>(lateUpdateManager);
        }

        async Awaitable InitializeObjects()
        {
            stateManager.Initialize();
            persistenceManager.Initialize();
            sceneLoaderManager.Initialize();
            updateManager.Initialize();
            fixedUpdateManager.Initialize();
            lateUpdateManager.Initialize();
            soundManager.Initialize();
            musicManager.Initialize();
            cullingManager.Initialize();

            await sceneLoaderManager.LoadSceneGroup(0);
        }

        async Awaitable CreateObjects() => await InstantiatePlayer();

        async Awaitable InstantiatePlayer()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(playerReference);

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                player = Instantiate(handle.Result);
            else
                Logging.LogError("Failed to load player asset: Player");
        }

        void PrepareGame()
        {
            DontDestroyOnLoad(player);
            player.transform.position = new Vector3(0f, 0f, -5f);
        }

        void BeginGame()
        {
            persistenceManager.NewGame(); //Only in tests
        }
    }
}
