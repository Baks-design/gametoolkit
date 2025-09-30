using Cysharp.Threading.Tasks;

namespace GameToolkit.Runtime.Application.Scenes
{
    public interface ISceneLoaderServices
    {
        UniTask LoadSceneGroup(int index);
    }
}
