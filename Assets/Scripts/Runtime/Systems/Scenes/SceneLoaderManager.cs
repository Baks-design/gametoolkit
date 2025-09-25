using GameToolkit.Runtime.Systems.Persistence;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class SceneLoaderManager : MonoBehaviour, ISceneLoaderServices
    {
        [SerializeField]
        SceneGroup[] sceneGroups;
        float targetProgress;
        PersistenceManager persistenceManager;
        readonly SceneGroupManager manager = new();

        public void Initialize(PersistenceManager persistenceManager)
        {
            this.persistenceManager = persistenceManager;
            DontDestroyOnLoad(gameObject);
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

            PersistenceBinding();
        }

        void PersistenceBinding()
        {
            persistenceManager.Bind<PlayerDataHandler, PlayerData>(
                persistenceManager.GameData.playerData
            );
        }
    }
}
