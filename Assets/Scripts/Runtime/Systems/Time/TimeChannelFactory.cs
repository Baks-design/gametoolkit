using System;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Times
{
    /// <summary>
    /// Provides factory methods to create time channels based on Unity's built-in time sources.
    /// </summary>
    public static class TimeChannelFactory
    {
        public static TimeChannel Create(SupportedTime type, float defaultTimeScale = 1f) =>
            new TimeChannel(type, GetProvider(type), defaultTimeScale);

        static Func<float> GetProvider(SupportedTime type) =>
            type switch
            {
                SupportedTime.DeltaTime => () => Time.deltaTime,
                SupportedTime.FixedDeltaTime => () => Time.fixedDeltaTime,
                SupportedTime.UnscaledDeltaTime => () => Time.unscaledDeltaTime,
                _
                    => throw new ArgumentOutOfRangeException(
                        nameof(type),
                        $"Unsupported time type: {type}"
                    )
            };
    }
}
