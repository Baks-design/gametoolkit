using UnityEngine;

namespace GameToolkit.Runtime.Application.Scenes
{
    public interface ISceneLoaderServices
    {
        Awaitable LoadSceneGroup(int index);
    }
}
