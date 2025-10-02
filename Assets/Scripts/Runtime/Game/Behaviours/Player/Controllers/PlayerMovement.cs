using Alchemy.Inspector;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Helpers;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerMovement
        : MonoBehaviour,
            ICrouchHandler,
            ICameraHandler,
            IVelocityHandler,
            IRunnningHandler,
            ILandingHandler,
            IJumpHandler,
            IDirectionHandler
    {
        [SerializeField, Required]
        Transform yawTransform;

        [SerializeField, Required]
        CharacterController controller;

        [SerializeField]
        InterfaceReference<IPlayerCamera> playerCamera;

        [SerializeField]
        InterfaceReference<IPlayerSound> playerSound;

        [SerializeField]
        InterfaceReference<IPlayerCollision> collision;

        [SerializeField]
        InterfaceReference<IPlayerAnimation> playerAnimation;

        [SerializeField]
        PlayerMovementConfig movementConfig;

        [SerializeField, InlineEditor]
        HeadBobData headBobConfig;

        [SerializeField, ReadOnly]
        PlayerMovementData movementData;
        VelocityHandler velocity;
        CameraHandler cam;
        CrouchHandler crouch;
        DirectionHandler direction;
        JumpHandler jump;
        LandingHandler landing;
        HeadBobHandler headBob;
        RunnningHandler runnning;
        IMovementInput movementInput;

        void Start()
        {
            ServiceLocator.Global.Get(out movementInput);

            var collisionData = new PlayerCollisionData();
            direction = new DirectionHandler(
                movementInput,
                controller,
                movementConfig,
                collisionData,
                movementData
            );
            jump = new JumpHandler(movementInput, collisionData, movementData, movementConfig);
            landing = new LandingHandler(
                yawTransform,
                collisionData,
                movementData,
                movementConfig,
                playerSound.Value,
                playerAnimation.Value
            );
            headBob = new HeadBobHandler(headBobConfig, movementData, movementConfig);
            runnning = new RunnningHandler(movementInput, controller, movementConfig, movementData);
            velocity = new VelocityHandler(
                movementInput,
                controller,
                movementConfig,
                movementData,
                runnning
            );
            cam = new CameraHandler(
                movementInput,
                controller,
                movementConfig,
                collisionData,
                headBob,
                movementData,
                playerCamera.Value,
                yawTransform,
                runnning
            );
            crouch = new CrouchHandler(
                movementInput,
                controller,
                yawTransform,
                movementConfig,
                collisionData,
                movementData,
                collision.Value
            );
        }

        #region IVelocityHandler
        public void CalculateSpeed() => velocity.CalculateSpeed();

        public void SmoothSpeed(float deltaTime) => velocity.SmoothSpeed(deltaTime);

        public void ApplyGravityOnGrounded() => velocity.ApplyGravityOnGrounded();

        public void ApplyGravityOnAirborne(float deltaTime) =>
            velocity.ApplyGravityOnAir(deltaTime);

        public void CalculateFinalAirborneAcceleration() =>
            velocity.CalculateFinalAccelerationOnAir();

        public void CalculateFinalGroundedAcceleration() =>
            velocity.CalculateFinalAccelerationOnGrounded();

        public void ApplyMove(float deltaTime) => velocity.ApplyMove(deltaTime);
        #endregion

        #region ICrouchHandler
        public void HandleCrouch(float deltaTime) => crouch.HandleCrouch(deltaTime);
        #endregion

        #region IJumpHandler
        public void HandleJump(float time)
        {
            jump.UpdateJumpBuffer(time);
            jump.HandleJump(time);
        }
        #endregion

        #region ILandingHandler
        public void HandleLanding(float deltaTime) => landing.HandleLanding(deltaTime);
        #endregion

        #region IRunnningHandler
        public void HandleRun() => runnning.HandleRun();
        #endregion

        #region ICameraHandler
        public void RotateTowardsCamera(float deltaTime) => cam.RotateTowardsCamera(deltaTime);

        public void HandleHeadBob(float deltaTime) => cam.HandleHeadBob(deltaTime);

        public void HandleCameraSway(float deltaTime) => cam.HandleCameraSway(deltaTime);

        public void HandleRunFOV(float deltaTime) => cam.HandleRunFOV(deltaTime);
        #endregion

        #region IDirectionHandler
        public void SmoothDirection(float deltaTime) => direction.SmoothDirection(deltaTime);

        public void SmoothInput(float deltaTime) => direction.SmoothInput(deltaTime);

        public void CalculateMovementAirborneDirection() =>
            direction.CalculateMovementDirectionOnAir();

        public void CalculateMovementGroundedDirection() =>
            direction.CalculateMovementDirectionOnGrounded();
        #endregion
    }
}
