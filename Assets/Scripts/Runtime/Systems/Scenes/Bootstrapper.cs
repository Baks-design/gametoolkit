using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public class Bootstrapper : MonoBehaviour
    {
        // NOTE: This script is intended to be placed in your first scene included in the build settings.
        static readonly int sceneIndex = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Logging.Log("Bootstrapper...");
#if UNITY_EDITOR
            // Set the bootstrapper scene to be the play mode start scene when running in the editor
            // This will cause the bootstrapper scene to be loaded first (and only once) when entering
            // play mode from the Unity Editor, regardless of which scene is currently active.
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
                EditorBuildSettings.scenes[sceneIndex].path
            );
#endif
        }

        void Awake() => DontDestroyOnLoad(gameObject);
    }
}
