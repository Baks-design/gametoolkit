using System;
using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public abstract class Timer : IDisposable
    {
        protected float initialTime;

        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public float Progress
        {
            get
            {
                if (CurrentTime <= 0f)
                    return 0f;
                if (CurrentTime >= initialTime)
                    return 1f;
                if (initialTime <= Mathf.Epsilon)
                    return 1f;

                return CurrentTime / initialTime;
            }
        }

        public Action OnTimerStart;
        public Action OnTimerStop;

        protected Timer(float value) => initialTime = value;

        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart?.Invoke();
            }
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop?.Invoke();
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Resume() => IsRunning = true;

        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        bool disposed;

        ~Timer()
        {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                TimerManager.DeregisterTimer(this);

            disposed = true;
        }
    }
}
