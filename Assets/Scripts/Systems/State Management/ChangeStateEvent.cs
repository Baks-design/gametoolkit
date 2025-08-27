using GameToolkit.Runtime.Utils.Tools.EventBus;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public struct ChangeStateEvent : IEvent
    {
        public bool IsPlaying;
    }
}
