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
        readonly ICrouchingHandler crouch;
        readonly ICameraHandler camHandler;
        readonly IVelocityHandler velocity;
        readonly IRunnningHandler run;
        readonly ILandingHandler land;
        readonly IJumpingHandler jump;
        readonly IDirectionHandler direction;

        public PlayerGroundedState(
            PlayerCollisionData collisionData,
            IPlayerSound sound,
            IPlayerAnimation animation,
            IPlayerCamera camera,
            IPlayerCollision collision,
            ICrouchingHandler crouch,
            ICameraHandler camHandler,
            IVelocityHandler velocity,
            IRunnningHandler run,
            ILandingHandler land,
            IJumpingHandler jump,
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

        public void Update(float deltaTime, float time)
        {
            // 1. Input and camera orientation first
            camHandler.RotateTowardsCamera(deltaTime);
            direction.SmoothInput(deltaTime);

            // 2. State checks
            collision.GroundCheckHandler();
            collision.ObstacleCheckHandler();

            // 3. Movement calculations
            direction.CalculateMovementGroundedDirection();
            direction.SmoothDirection(deltaTime);
            velocity.CalculateSpeed();
            velocity.SmoothSpeed(deltaTime);
            velocity.CalculateFinalAcceleration();

            // 4. State handlers (running, crouching, jumping)
            run.HandleRunning();
            crouch.HandleCrouching(deltaTime);
            jump.HandleJumping(time);

            // 5. Physics and movement
            velocity.ApplyGravityOnGrounded();
            velocity.ApplyMove(deltaTime);

            // 6. Update collision data (do this before animations/sounds)
            collisionData.PreviouslyGrounded = collisionData.OnGrounded;

            // 7. Camera effects (that depend on movement)
            camHandler.HandleHeadBob(deltaTime);
            camHandler.HandleRunFOV(deltaTime);
            camHandler.HandleCameraSway(deltaTime);
            land.HandleLanding(deltaTime);

            // 8. Animations (after all state changes)
            animation.UpdateMoving();
            animation.UpdateCrouch();
            animation.UpdateJump();

            // 9. Sounds (after animations/state changes)
            sound.UpdateFootsteps(deltaTime);
            sound.UpdateJumping();
            sound.UpdateLanding();
        }

        public void LateUpdate(float deltaTime)
        {
            camera.BreathingHandler(deltaTime);
            camera.RotationHandler(deltaTime);
            camera.AimHandler(deltaTime);
        }
    }
}
