using UnityEngine;

namespace GameToolkit.Runtime.Utils.Extensions
{
    public static class FloatExtensions
    {
        public static float ClampAngle(this float angle, float from, float to)
        {
            if (angle < 0f)
                angle = 360 + angle;

            if (angle > 180f)
                return Mathf.Max(angle, 360f + from);

            return Mathf.Min(angle, to);
        }
    }
}
