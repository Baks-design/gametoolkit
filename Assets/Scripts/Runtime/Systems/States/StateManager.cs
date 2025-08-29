using GameToolkit.Runtime.Systems.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.EventBus;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Systems.StateManagement
{
    public class StateManager : StatefulEntity, IStateServices
    {
        [SerializeField]
        InputManager inputManager;
        EventBinding<ChangeStateEvent> changeEventBinding;
        bool IsPlaying = true;

        public IState CurrentState => stateMachine.CurrentState;

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
            var playState = new PlayState(inputManager);
            var pauseState = new PauseState(inputManager);

            At(pauseState, playState, IsPlaying);
            At(playState, pauseState, !IsPlaying);

            stateMachine.SetState(playState);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
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
    }
}
