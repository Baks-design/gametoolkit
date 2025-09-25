using GameToolkit.Runtime.Systems.SceneManagement;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class StateManager : StatefulEntity, IStateServices
    {
        EventBinding<PauseScreenEvent> pauseScreenEventBinding;

        public bool IsGameRunning { get; private set; }

        public void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            SetupStateMachine();
        }

        void SetupStateMachine()
        {
            var gameplayState = new GameplayState();
            var pauseState = new PauseState();

            At(gameplayState, pauseState, !IsGameRunning);
            At(pauseState, gameplayState, IsGameRunning);

            stateMachine.SetState(gameplayState);
        }

        void OnEnable()
        {
            pauseScreenEventBinding = new EventBinding<PauseScreenEvent>(HandlePauseScreenEvent);
            EventBus<PauseScreenEvent>.Register(pauseScreenEventBinding);
        }

        void HandlePauseScreenEvent(PauseScreenEvent pauseScreenEvent)
        {
            Logging.Log($"Current State: {pauseScreenEvent.HasOpened}");
            IsGameRunning = pauseScreenEvent.HasOpened;
        }

        void OnDisable() => EventBus<PauseScreenEvent>.Deregister(pauseScreenEventBinding);
    }
}
