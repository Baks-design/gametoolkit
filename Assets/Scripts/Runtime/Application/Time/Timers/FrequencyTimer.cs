using System;
using UnityEngine;

namespace GameToolkit.Runtime.Application.Time
{
    public class FrequencyTimer : Timer
    {
        float timeThreshold;

        public int TicksPerSecond { get; private set; }

        public Action OnTick;

        public FrequencyTimer(int ticksPerSecond)
            : base(0f) => CalculateTimeThreshold(ticksPerSecond);

        public override void Tick()
        {
            if (IsRunning && CurrentTime >= timeThreshold)
            {
                CurrentTime -= timeThreshold;
                OnTick?.Invoke();
            }

            if (IsRunning && CurrentTime < timeThreshold)
                CurrentTime += UnityEngine.Time.deltaTime;
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset() => CurrentTime = 0f;

        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = Mathf.Max(1, ticksPerSecond);
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}
