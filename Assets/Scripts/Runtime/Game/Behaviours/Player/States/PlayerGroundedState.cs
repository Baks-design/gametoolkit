using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerGroundedState : IState
    {
        readonly PlayerCollisionData collisionData;
        readonly IPlayerSound sound;
        readonly IPlayerAnimation animation;
        readonly IPlayerCamera camera;
        readonly IPlayerCollision collision;
        readonly ICrouchHandler crouch;
        readonly ICameraHandler camHandler;
        readonly IVelocityHandler velocity;
        readonly IRunnningHandler run;
        readonly ILandingHandler land;
        readonly IJumpHandler jump;
        readonly IDirectionHandler direction;

        public PlayerGroundedState(
            PlayerCollisionData collisionData,
            IPlayerSound sound,
            IPlayerAnimation animation,
            IPlayerCamera camera,
            IPlayerCollision collision,
            ICrouchHandler crouch,
            ICameraHandler camHandler,
            IVelocityHandler velocity,
            IRunnningHandler run,
            ILandingHandler land,
            IJumpHandler jump,
            IDirectionHandler direction
        )
        {
            this.collisionData = collisionData;
            this.sound = sound;
            this.animation = animation;
            this.camera = camera;
            this.collision = collision;
            this.crouch = crouch;
            this.camHandler = camHandler;
            this.velocity = velocity;
            this.run = run;
            this.land = land;
            this.jump = jump;
            this.direction = direction;
        }

        public void OnEnter() => Logging.Log("Enter in Grounded State");

        public void FixedUpdate(float deltaTime) { }

        public void Update(float deltaTime)
        {
            Logging.Log($"Current State: On Grounded State");

            camHandler.RotateTowardsCamera(deltaTime);

            collision.GroundCheckHandler();
            collision.ObstacleCheckHandler();

            direction.SmoothInput(deltaTime);
            velocity.SmoothSpeed(deltaTime);
            direction.SmoothDirection(deltaTime);

            direction.CalculateMovementGroundedDirection();
            velocity.CalculateSpeed();
            velocity.CalculateFinalGroundedAcceleration();

            //land.HandleLanding(deltaTime);
            run.HandleRun();
            crouch.HandleCrouch(deltaTime);
            camHandler.HandleHeadBob(deltaTime);
            camHandler.HandleRunFOV(deltaTime);
            camHandler.HandleCameraSway(deltaTime);

            velocity.ApplyGravityOnGrounded();
            jump.HandleJump();
            velocity.ApplyMove(deltaTime);

            collisionData.PreviouslyGrounded = collisionData.OnGrounded;

            // animation.UpdateMoving();
            // animation.UpdateCrouch();
            // animation.UpdateJump();

            sound.UpdateFootsteps(deltaTime);
            sound.UpdateLanding();
        }

        public void LateUpdate(float deltaTime)
        {
            camera.BreathingHandler(deltaTime);
            camera.AimHandler(deltaTime);
            camera.RotationHandler(deltaTime);
        }
    }
}
