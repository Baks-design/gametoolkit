namespace GameToolkit.Runtime.Application.Time
{
    public class CountdownTimer : Timer
    {
        public override bool IsFinished => CurrentTime <= 0f;

        public CountdownTimer(float value)
            : base(value) { }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0f)
                CurrentTime -= UnityEngine.Time.deltaTime;

            if (IsRunning && CurrentTime <= 0f)
                Stop();
        }
    }
}
