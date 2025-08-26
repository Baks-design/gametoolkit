using UnityEngine;

namespace GameToolkit.Runtime.Utils.Tools.StatesMachine
{
    public class StopwatchTimer : Timer
    {
        public override bool IsFinished => false;

        public StopwatchTimer()
            : base(0f) { }

        public override void Tick()
        {
            if (!IsRunning)
                return;
            CurrentTime += Time.deltaTime;
        }
    }
}
