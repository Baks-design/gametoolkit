using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerAirborneState : IState
    {
        readonly PlayerCollisionData collisionData;
        readonly IPlayerAnimation animation;
        readonly IPlayerCamera camera;
        readonly IPlayerCollision collision;
        readonly ICameraHandler camHandler;
        readonly IVelocityHandler velocity;
        readonly IDirectionHandler direction;
        private readonly IJumpingHandler jumping;

        public PlayerAirborneState(
            PlayerCollisionData collisionData,
            IPlayerAnimation animation,
            IPlayerCamera camera,
            IPlayerCollision collision,
            ICameraHandler camHandler,
            IVelocityHandler velocity,
            IDirectionHandler direction,
            IJumpingHandler jumping
        )
        {
            this.collisionData = collisionData;
            this.animation = animation;
            this.camera = camera;
            this.collision = collision;
            this.camHandler = camHandler;
            this.velocity = velocity;
            this.direction = direction;
            this.jumping = jumping;
        }

        public void OnEnter() => Logging.Log("Enter in Airborne State");

        public void Update(float deltaTime, float time)
        {
            // 1. Input and orientation first
            camHandler.RotateTowardsCamera(deltaTime);
            direction.SmoothInput(deltaTime);

            // 2. State checks
            collision.GroundCheckHandler();
            collision.ObstacleCheckHandler();

            // 3. Movement calculations for airborne
            jumping.UpdateJumpBuffer(time);
            direction.CalculateMovementAirborneDirection();
            direction.SmoothDirection(deltaTime);
            velocity.CalculateSpeed();
            velocity.SmoothSpeed(deltaTime);
            velocity.CalculateFinalAcceleration();

            // 4. Physics - gravity first, then movement
            velocity.ApplyGravityOnAirborne(deltaTime);
            velocity.ApplyMove(deltaTime);

            // 5. Update collision data IMMEDIATELY after physics
            collisionData.PreviouslyGrounded = collisionData.OnGrounded;

            // 6. Animations (after all physics and state changes)
            animation.UpdateJump();
        }

        public void LateUpdate(float deltaTime)
        {
            camera.BreathingHandler(deltaTime);
            camera.RotationHandler(deltaTime);
            camera.AimHandler(deltaTime);
        }
    }
}
