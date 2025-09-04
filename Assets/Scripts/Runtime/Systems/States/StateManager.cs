using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class StateManager : MonoBehaviour, IStateServices
    {
        EventBinding<ChangeStateEvent> changeEventBinding;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IStateServices>(this);
        }

        void OnEnable()
        {
            changeEventBinding = new EventBinding<ChangeStateEvent>(HandleStateEvent);
            EventBus<ChangeStateEvent>.Register(changeEventBinding);
        }

        void HandleStateEvent(ChangeStateEvent changeStateEvent)
        {
            Logging.Log($"Current State: {changeStateEvent.IsPlaying}");
        }

        void OnDisable()
        {
            EventBus<ChangeStateEvent>.Deregister(changeEventBinding);
        }
    }
}
