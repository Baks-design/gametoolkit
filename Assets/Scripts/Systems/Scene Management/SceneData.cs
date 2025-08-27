using System;
using Eflatun.SceneReference;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;

        public string Name => Reference.Name;
    }
}
