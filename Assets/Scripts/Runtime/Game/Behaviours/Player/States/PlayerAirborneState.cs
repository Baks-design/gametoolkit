using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerAirborneState : IState
    {
        readonly PlayerCollisionData collisionData;
        readonly IPlayerSound sound;
        readonly IPlayerAnimation animation;
        readonly IPlayerCamera camera;
        readonly IPlayerCollision collision;
        readonly ICameraHandler camHandler;
        readonly IVelocityHandler velocity;
        readonly IDirectionHandler direction;

        public PlayerAirborneState(
            PlayerCollisionData collisionData,
            IPlayerSound sound,
            IPlayerAnimation animation,
            IPlayerCamera camera,
            IPlayerCollision collision,
            ICameraHandler camHandler,
            IVelocityHandler velocity,
            IDirectionHandler direction
        )
        {
            this.collisionData = collisionData;
            this.sound = sound;
            this.animation = animation;
            this.camera = camera;
            this.collision = collision;
            this.camHandler = camHandler;
            this.velocity = velocity;
            this.direction = direction;
        }

        public void OnEnter() => Logging.Log("Enter in Airborne State");

        public void FixedUpdate(float deltaTime) { }

        public void Update(float deltaTime)
        {
            Logging.Log($"Current State: On Airborne State");

            camHandler.RotateTowardsCamera(deltaTime);

            collision.GroundCheckHandler();
            collision.ObstacleCheckHandler();

            direction.SmoothInput(deltaTime);
            velocity.SmoothSpeed(deltaTime);
            direction.SmoothDirection(deltaTime);

            direction.CalculateMovementAirborneDirection();
            velocity.CalculateSpeed();
            velocity.CalculateFinalAirborneAcceleration();

            velocity.ApplyGravityOnAirborne(deltaTime);
            velocity.ApplyMove(deltaTime);

            collisionData.PreviouslyGrounded = collisionData.OnGrounded;

            animation.UpdateFalling();
        }

        public void LateUpdate(float deltaTime)
        {
            camera.BreathingHandler(deltaTime);
            camera.RotationHandler(deltaTime);
            camera.AimHandler(deltaTime);
        }
    }
}
