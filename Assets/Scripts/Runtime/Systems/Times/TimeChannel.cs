using System;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.Times
{
    /// <summary>
    /// Represents a single time channel with an adjustable time scale.
    /// </summary>
    public class TimeChannel
    {
        readonly Func<float> baseTimeProvider;
        float timeScale;

        public SupportedTime Type { get; }

        /// <summary>
        /// Gets the delta time adjusted by the channel's time scale.
        /// /// </summary>
        public float DeltaTime => baseTimeProvider() * TimeScale;
        public float TimeScale
        {
            get => timeScale;
            set
            {
                if (Mathf.Approximately(timeScale, value))
                    return; // command this line if you want to trigger callback with same value.
                timeScale = value;
                OnTimeScaleChanged?.Invoke(timeScale);
            }
        }

        public event Action<float> OnTimeScaleChanged;

        public TimeChannel(SupportedTime type, Func<float> baseTimeProvider, float timeScale = 1f)
        {
            this.baseTimeProvider = baseTimeProvider;
            Type = type;
            TimeScale = timeScale;
        }
    }
}
