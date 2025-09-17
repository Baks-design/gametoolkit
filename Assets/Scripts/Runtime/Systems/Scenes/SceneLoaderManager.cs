using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class SceneLoaderManager : CustomMonoBehaviour, ISceneLoaderServices
    {
        [SerializeField]
        SceneGroup[] sceneGroups;
        float targetProgress;
        readonly SceneGroupManager manager = new();

        public void Initialize()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Global.Register<ISceneLoaderServices>(this);
        }

        public async Awaitable LoadSceneGroup(int index)
        {
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Logging.LogError($"Invalid scene group index: {index}");
                return;
            }

            var progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            await manager.LoadScenes(sceneGroups[index], progress);
        }
    }
}
