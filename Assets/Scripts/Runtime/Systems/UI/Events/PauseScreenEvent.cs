using GameToolkit.Runtime.Utils.Tools.EventBus;

namespace GameToolkit.Runtime.Systems.SceneManagement
{
    public struct PauseScreenEvent : IEvent
    {
        public bool HasOpened;
    }
}
