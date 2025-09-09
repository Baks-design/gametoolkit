using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class SceneLoaderManager : CustomMonoBehaviour, ISceneLoaderServices
    {
        [SerializeField]
        Image loadingBar;

        [SerializeField]
        float fillSpeed = 0.5f;

        [SerializeField]
        Canvas loadingCanvas;

        [SerializeField]
        CinemachineCamera loadingCamera;

        [SerializeField]
        SceneGroup[] sceneGroups;

        float targetProgress;
        bool isLoading;
        public readonly SceneGroupManager manager = new();

        protected override void Awake()
        {
            base.Awake();
            Setup();
            Logger();
        }

        void Setup()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Global.Register<ISceneLoaderServices>(this);
        }

        void Logger()
        {
            //manager.OnSceneLoaded += sceneName => Logging.Log($"Loaded: {sceneName}");
            //manager.OnSceneUnloaded += sceneName => Logging.Log($"Unloaded: {sceneName}");
            //manager.OnSceneGroupLoaded += () => Logging.Log("Scene group loaded");
        }

        protected override async void Start() => await LoadSceneGroup(0);

        public async Awaitable LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Logging.LogError($"Invalid scene group index: {index}");
                return;
            }

            var progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }

        public override void ProcessUpdate(float deltaTime)
        {
            if (!isLoading)
                return;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
            var dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(
                currentFillAmount,
                targetProgress,
                deltaTime * dynamicFillSpeed
            );
        }
    }
}
