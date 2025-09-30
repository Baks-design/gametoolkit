using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace GameToolkit.Runtime.Application.Scenes
{
    public class SceneGroupManager
    {
        readonly AsyncOperationHandleGroup handleGroup = new(10);
        SceneGroup ActiveSceneGroup;

        public event Action<string> OnSceneLoaded;
        public event Action<string> OnSceneUnloaded;
        public event Action OnSceneGroupLoaded;

        public async UniTask LoadScenes(
            SceneGroup group,
            IProgress<float> progress,
            bool reloadDupScenes = false
        )
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name))
                    continue;

                if (sceneData.Reference.State == SceneReferenceState.Regular)
                {
                    var operation = SceneManager.LoadSceneAsync(
                        sceneData.Reference.Path,
                        LoadSceneMode.Additive
                    );
                    operationGroup.Operations.Add(operation);
                }
                else if (sceneData.Reference.State == SceneReferenceState.Addressable)
                {
                    var sceneHandle = Addressables.LoadSceneAsync(
                        sceneData.Reference.Path,
                        LoadSceneMode.Additive
                    );
                    handleGroup.Handles.Add(sceneHandle);
                }

                OnSceneLoaded?.Invoke(sceneData.Name);
            }

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone || !handleGroup.IsDone)
            {
                progress?.Report((operationGroup.Progress + handleGroup.Progress) / 2f);
                await UniTask.WaitForSeconds(0.1f);
            }

            var activeScene = SceneManager.GetSceneByName(
                ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene)
            );

            if (activeScene.IsValid())
                SceneManager.SetActiveScene(activeScene);

            OnSceneGroupLoaded?.Invoke();
        }

        public async UniTask UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            var sceneCount = SceneManager.sceneCount;
            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded)
                    continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper")
                    continue;

                var hasValidHandle = false;
                foreach (var handle in handleGroup.Handles)
                {
                    if (handle.IsValid() && handle.Result.Scene.name == sceneName)
                    {
                        hasValidHandle = true;
                        break;
                    }
                }
                if (hasValidHandle)
                    continue;

                scenes.Add(sceneName);
            }

            // Create an AsyncOperationGroup
            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null)
                    continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded?.Invoke(scene);
            }

            foreach (var handle in handleGroup.Handles)
                if (handle.IsValid())
                    await Addressables.UnloadSceneAsync(handle);

            handleGroup.Handles.Clear();

            // Wait until all AsyncOperations in the group are done
            while (!operationGroup.IsDone)
                await UniTask.WaitForSeconds(0.1f); // delay to avoid tight loop
        }
    }
}
