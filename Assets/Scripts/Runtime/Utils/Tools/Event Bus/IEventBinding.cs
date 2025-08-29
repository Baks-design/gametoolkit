using System;

namespace GameToolkit.Runtime.Utils.Tools.EventBus
{
    public interface IEventBinding<T>
    {
        Action<T> OnEvent { get; set; }
        Action OnEventNoArgs { get; set; }
    }
}
