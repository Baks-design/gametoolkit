using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerClimbingState : IState
    {
        readonly PlayerMovement playerMovement;

        public PlayerClimbingState(PlayerMovement playerMovement) =>
            this.playerMovement = playerMovement;

        public void OnEnter() { }

        public void FixedUpdate(float deltaTime) { }

        public void Update(float deltaTime) { }

        public void LateUpdate(float deltaTime) { }

        public void OnExit() { }
    }
}
