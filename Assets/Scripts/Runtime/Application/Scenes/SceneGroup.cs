using System;
using System.Collections.Generic;

namespace GameToolkit.Runtime.Application.Scenes
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        public string FindSceneNameByType(SceneType sceneType)
        {
            foreach (var scene in Scenes)
                if (scene.SceneType == sceneType)
                    return scene.Reference?.Name;
            return null;
        }
    }
}
