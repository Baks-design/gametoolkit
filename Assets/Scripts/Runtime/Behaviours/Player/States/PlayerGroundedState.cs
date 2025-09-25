using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerGroundedState : IState
    {
        readonly CameraHandler cameraHandler;
        readonly CrouchHandler crouchHandler;
        readonly DirectionHandler directionHandler;
        readonly JumpHandler jumpHandler;
        readonly LandingHandler landingHandler;
        readonly VelocityHandler velocityHandler;
        readonly RunnningHandler runnningHandler;

        public PlayerGroundedState(
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

        public void OnEnter() => Logging.Log("Enter in Grounded State");

        public void Update(float deltaTime)
        {
            Logging.Log($"Current State: Grounded State");
            //Logging.Log($"Delta Time:{deltaTime}");

            cameraHandler.RotateTowardsCamera(deltaTime);

            // Apply Smoothing
            directionHandler.SmoothInput(deltaTime);
            velocityHandler.SmoothSpeed(deltaTime);
            directionHandler.SmoothDirection(deltaTime);

            // Calculate Movement
            directionHandler.CalculateMovementGroundedDirection();
            velocityHandler.CalculateSpeed();
            velocityHandler.CalculateFinalGroundedAcceleration();

            // Handle Player Movement, Gravity, Jump, Crouch etc.
            runnningHandler.HandleRun();
            crouchHandler.HandleCrouch(deltaTime);
            cameraHandler.HandleHeadBob(deltaTime);
            cameraHandler.HandleRunFOV(deltaTime);
            cameraHandler.HandleCameraSway(deltaTime);

            // Apply Movement
            velocityHandler.ApplyGravityOnGrounded();
            jumpHandler.HandleJump();
            velocityHandler.ApplyMove(deltaTime);
        }
    }
}
