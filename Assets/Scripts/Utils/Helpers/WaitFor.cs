using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Helpers
{
    public static class WaitFor
    {
        static readonly Dictionary<float, WaitForSeconds> waitForSecondsDict =
            new(100, new FloatComparer());
        static readonly WaitForFixedUpdate fixedUpdate = new();
        static readonly WaitForEndOfFrame endOfFrame = new();
        static readonly Dictionary<object, WaitWhile> waitWhileCache = new();

        public static WaitForFixedUpdate FixedUpdate => fixedUpdate;
        public static WaitForEndOfFrame EndOfFrame => endOfFrame;

        public static WaitForSeconds GetSeconds(float seconds)
        {
            if (seconds < Time.unscaledDeltaTime)
                return null;

            if (!waitForSecondsDict.TryGetValue(seconds, out var waitForSeconds))
            {
                waitForSeconds = new WaitForSeconds(seconds);
                waitForSecondsDict[seconds] = waitForSeconds;
            }

            return waitForSeconds;
        }

        public static WaitWhile GetWaitWhile(Func<bool> condition)
        {
            Checking.AgainstNull(condition, nameof(condition));

            if (!waitWhileCache.TryGetValue(condition, out var waitWhile))
            {
                waitWhile = new WaitWhile(condition);
                waitWhileCache[condition] = waitWhile;
            }

            return waitWhile;
        }

        class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Mathf.Epsilon;

            public int GetHashCode(float obj) => obj.GetHashCode();
        }
    }
}
