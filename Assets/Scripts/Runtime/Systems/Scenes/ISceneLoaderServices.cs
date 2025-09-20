using UnityEngine;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public interface ISceneLoaderServices
    {
        Awaitable LoadSceneGroup(int index);
    }
}
