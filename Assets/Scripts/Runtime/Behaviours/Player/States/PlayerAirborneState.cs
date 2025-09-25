using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerAirborneState : IState
    {
        readonly CameraHandler cameraHandler;
        readonly CrouchHandler crouchHandler;
        readonly DirectionHandler directionHandler;
        readonly LandingHandler landingHandler;
        readonly VelocityHandler velocityHandler;
        readonly RunnningHandler runnningHandler;

        public PlayerAirborneState(
            CameraHandler cameraHandler,
            CrouchHandler crouchHandler,
            DirectionHandler directionHandler,
            LandingHandler landingHandler,
            VelocityHandler velocityHandler,
            RunnningHandler runnningHandler
        )
        {
            this.cameraHandler = cameraHandler;
            this.crouchHandler = crouchHandler;
            this.directionHandler = directionHandler;
            this.landingHandler = landingHandler;
            this.velocityHandler = velocityHandler;
            this.runnningHandler = runnningHandler;
        }

        public void OnEnter() => Logging.Log("Enter in Airborne State");

        public void Update(float deltaTime)
        {
            Logging.Log($"Current State: PlayerAirborneState State");
            //Logging.Log($"Delta Time:{deltaTime}");

            cameraHandler.RotateTowardsCamera(deltaTime);

            // Apply Smoothing
            directionHandler.SmoothInput(deltaTime);
            velocityHandler.SmoothSpeed(deltaTime);
            directionHandler.SmoothDirection(deltaTime);

            // Calculate Movement
            directionHandler.CalculateMovementAirborneDirection();
            velocityHandler.CalculateSpeed();
            velocityHandler.CalculateFinalAirborneAcceleration();

            // Handle Player Movement, Gravity, Jump, Crouch etc.
            runnningHandler.HandleRun();
            cameraHandler.HandleCameraSway(deltaTime);
            landingHandler.HandleLanding(deltaTime);

            // Apply Movement
            velocityHandler.ApplyGravityOnAirborne(deltaTime);
            velocityHandler.ApplyMove(deltaTime);
        }
    }
}
