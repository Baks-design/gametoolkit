using System;

namespace GameToolkit.Runtime.Application.Scenes
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        public void Report(float value) => Progressed?.Invoke(value / 1f);
    }
}
