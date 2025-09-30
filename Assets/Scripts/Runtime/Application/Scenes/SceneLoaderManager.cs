using Cysharp.Threading.Tasks;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Application.Scenes
{
    public class SceneLoaderManager : MonoBehaviour, ISceneLoaderServices
    {
        [SerializeField]
        SceneGroup[] sceneGroups;
        float targetProgress;
        readonly SceneGroupManager manager = new();

        public void Initialize() => DontDestroyOnLoad(gameObject);

        public async UniTask LoadSceneGroup(int index)
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
