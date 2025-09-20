using GameToolkit.Runtime.Behaviours.Player;
using GameToolkit.Runtime.Systems.Audio;
using GameToolkit.Runtime.Systems.Culling;
using GameToolkit.Runtime.Systems.Persistence;
using GameToolkit.Runtime.Systems.StateManagement;
using GameToolkit.Runtime.Systems.UpdateManagement;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        CinemachineBrain cinemachineBrain;

        [SerializeField]
        Light directionalLight;

        [SerializeField]
        EventSystem eventSystem;

        [SerializeField]
        LoadingScreen loadingScreen;

        [SerializeField]
        PlayerMovementController playerController;

        [Header("Managers")]
        [SerializeField]
        SceneLoaderManager sceneLoaderManager;

        [SerializeField]
        PersistenceManager persistenceManager;

        [SerializeField]
        StateManager stateManager;

        [SerializeField]
        UpdateManager updateManager;

        [SerializeField]
        FixedUpdateManager fixedUpdateManager;

        [SerializeField]
        LateUpdateManager lateUpdateManager;

        [SerializeField]
        SoundManager soundManager;

        [SerializeField]
        MusicManager musicManager;

        [SerializeField]
        CullingManager cullingManager;

        async void Awake()
        {
            BindObjects();

            using var loadingScreenDisposable = new ShowLoadingScreenDisposable(loadingScreen);
            loadingScreenDisposable.SetLoadingBarPercent(0f);
            await InitializeObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.33f);
            //await CreateObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.66f);
            PrepareGame();
            loadingScreenDisposable.SetLoadingBarPercent(1f);

            //await BeginGame();
        }

        void BindObjects()
        {
            cinemachineBrain = Instantiate(cinemachineBrain);
            directionalLight = Instantiate(directionalLight);
            eventSystem = Instantiate(eventSystem);
            sceneLoaderManager = Instantiate(sceneLoaderManager);
            persistenceManager = Instantiate(persistenceManager);
            stateManager = Instantiate(stateManager);
            updateManager = Instantiate(updateManager);
            fixedUpdateManager = Instantiate(fixedUpdateManager);
            lateUpdateManager = Instantiate(lateUpdateManager);
            soundManager = Instantiate(soundManager);
            musicManager = Instantiate(musicManager);
            cullingManager = Instantiate(cullingManager);
            playerController = Instantiate(playerController);
            loadingScreen = Instantiate(loadingScreen);
        }

        async Awaitable InitializeObjects()
        {
            sceneLoaderManager.Initialize();
            await sceneLoaderManager.LoadSceneGroup(0);
        }

        // async Awaitable CreateObjects()
        // {
        //     await HEAVY OBJECTS;
        // }

        void PrepareGame()
        {
            playerController.transform.position = new Vector3(0f, 0f, -5f);
        }

        // async Awaitable BeginGame()
        // {
        //     await levelUI.ShowLevelAnimation();
        // }
    }
}
