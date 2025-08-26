using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    internal static class TimerBootstrapper
    {
        static PlayerLoopSystem timerSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0))
            {
                Logging.LogWarning(
                    "Improved Timers not initialized, unable to register TimerManager into the Update loop."
                );
                return;
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;

            static void OnPlayModeState(PlayModeStateChange state)
            {
                if (state is PlayModeStateChange.ExitingPlayMode)
                {
                    var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveTimerManager<Update>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    TimerManager.Clear();
                }
            }
#endif
        }

        static void RemoveTimerManager<T>(ref PlayerLoopSystem loop) =>
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in timerSystem);

        static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
        {
            timerSystem = new PlayerLoopSystem()
            {
                type = typeof(TimerManager),
                updateDelegate = TimerManager.UpdateTimers,
                subSystemList = null
            };
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index);
        }
    }
}
