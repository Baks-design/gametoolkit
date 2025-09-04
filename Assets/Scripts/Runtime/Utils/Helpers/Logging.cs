using Debug = UnityEngine.Debug;
using Diagnostics = System.Diagnostics;
using Exception = System.Exception;
using Object = UnityEngine.Object;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class Logging
    {
        [Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message, Object context = null) =>
            Debug.Log(message, context);

        [Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(object message, Object context = null) =>
            Debug.LogError(message, context);

        [Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogWarning(object message, Object context = null) =>
            Debug.LogWarning(message, context);

        [Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogException(Exception exception, Object context = null) =>
            Debug.LogException(exception, context);

        [Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogErrorFormat(string format, params object[] args) =>
            Debug.LogErrorFormat(format, args);
    }
}
