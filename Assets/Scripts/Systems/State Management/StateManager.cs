using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class StateManager : StatefulEntity, IStateServices
    {
        EventBinding<ChangeStateEvent> changeEventBinding;

        public IState CurrentState => stateMachine.CurrentState;
        public bool IsPlaying { get; private set; } = true;

        protected override void Awake()
        {
            base.Awake();
            Setup();
            StateMachineSetup();
        }

        void Setup()
        {
            DontDestroyOnLoad(gameObject);
            ServiceLocator.Global.Register<IStateServices>(this);
        }

        void StateMachineSetup()
        {
            base.Awake();

            var playState = new PlayState(this);
            var pauseState = new PauseState(this);

            At(pauseState, playState, IsPlaying);
            At(playState, pauseState, !IsPlaying);

            stateMachine.SetState(playState);
        }

        protected override void Start()
        {
            base.Start();
            changeEventBinding = new EventBinding<ChangeStateEvent>(HandleStateEvent);
            EventBus<ChangeStateEvent>.Register(changeEventBinding);
        }

        void HandleStateEvent(ChangeStateEvent changeStateEvent)
        {
            Logging.Log($"Current State: {changeStateEvent.IsPlaying}");
            IsPlaying = changeStateEvent.IsPlaying;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<ChangeStateEvent>.Deregister(changeEventBinding);
        }

        public void ChangeState(IState state) => stateMachine.SetState(state);
    }
}
