using System;
using UnityEngine;

namespace GameToolkit.Runtime.Application.Time
{
    public class TimeChannelSignal : MonoBehaviour
    {
        [SerializeField]
        SupportedTime timeType;

        [SerializeField]
        float initTimeScale = 1f;

        /// <summary>
        /// ID to register a unique channel for this signal.
        /// </summary>
        public string SignalId { get; private set; }

        /// <summary>
        /// A time channel register on awake using unique id.
        /// This channel will unregister on destroy.
        /// </summary>
        public TimeChannel Channel { get; private set; }

        void Awake()
        {
            SignalId = Guid.NewGuid().ToString();
            Channel = TimeChannelManager.Register(SignalId, timeType, initTimeScale);
        }

        void OnDestroy() => TimeChannelManager.Unregister(SignalId);
    }
}
