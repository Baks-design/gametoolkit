using System;
using Eflatun.SceneReference;

namespace GameToolkit.Runtime.Application.Scenes
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;

        public string Name => Reference.Name;
    }
}
