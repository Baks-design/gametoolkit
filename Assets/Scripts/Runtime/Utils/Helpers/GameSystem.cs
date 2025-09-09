using UnityEngine;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class GameSystem
    {
        public static void SetCursor(bool isLocked) =>
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;

        public static void SetTimeScale(float value) => Time.timeScale = value;
    }
}
