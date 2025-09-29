using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerClimbingState : IState
    {
        readonly CameraHandler cameraHandler;
        readonly CrouchHandler crouchHandler;
        readonly DirectionHandler directionHandler;
        readonly JumpHandler jumpHandler;
        readonly LandingHandler landingHandler;
        readonly VelocityHandler velocityHandler;
        readonly RunnningHandler runnningHandler;

        public PlayerClimbingState(
            CameraHandler cameraHandler,
            CrouchHandler crouchHandler,
            DirectionHandler directionHandler,
            JumpHandler jumpHandler,
            LandingHandler landingHandler,
            VelocityHandler velocityHandler,
            RunnningHandler runnningHandler
        )
        {
            this.cameraHandler = cameraHandler;
            this.crouchHandler = crouchHandler;
            this.directionHandler = directionHandler;
            this.jumpHandler = jumpHandler;
            this.landingHandler = landingHandler;
            this.velocityHandler = velocityHandler;
            this.runnningHandler = runnningHandler;
        }

        public void OnEnter() => Logging.Log("Enter in Climbing State");

        public void Update(float deltaTime)
        {
            //Logging.Log($"Current State:{Climbing State}");
        }
    }
}
