using System;

namespace GameToolkit.Runtime.Application.Time
{
    /// <summary>
    /// Provides factory methods to create time channels based on Unity's built-in time sources.
    /// </summary>
    public static class TimeChannelFactory
    {
        public static TimeChannel Create(SupportedTime type, float defaultTimeScale = 1f) =>
            new(type, GetProvider(type), defaultTimeScale);

        static Func<float> GetProvider(SupportedTime type) =>
            type switch
            {
                SupportedTime.DeltaTime => () => UnityEngine.Time.deltaTime,
                SupportedTime.FixedDeltaTime => () => UnityEngine.Time.fixedDeltaTime,
                SupportedTime.UnscaledDeltaTime => () => UnityEngine.Time.unscaledDeltaTime,
                _
                    => throw new ArgumentOutOfRangeException(
                        nameof(type),
                        $"Unsupported time type: {type}"
                    )
            };
    }
}
