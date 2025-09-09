using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class StateManager : StatefulEntity, IStateServices
    {
        EventBinding<ChangeStateEvent> changeEventBinding;

        public bool IsGameRunning { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SetupManager();
            SetupStateMachine();
        }

        void SetupManager()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IStateServices>(this);
        }

        void SetupStateMachine()
        {
            var gameplayState = new GameplayState();
            var pauseState = new PauseState();

            At(gameplayState, pauseState, !IsGameRunning);
            At(pauseState, gameplayState, IsGameRunning);

            stateMachine.SetState(gameplayState);
        }

        protected override void OnEnable()
        {
            changeEventBinding = new EventBinding<ChangeStateEvent>(HandleStateEvent);
            EventBus<ChangeStateEvent>.Register(changeEventBinding);
        }

        void HandleStateEvent(ChangeStateEvent changeStateEvent)
        {
            Logging.Log($"Current State: {changeStateEvent.IsPlaying}");
            IsGameRunning = changeStateEvent.IsPlaying;
        }

        protected override void OnDisable() =>
            EventBus<ChangeStateEvent>.Deregister(changeEventBinding);
    }
}
