using Alchemy.Inspector;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerMovementController : StatefulEntity, IUpdatable
    {
        [SerializeField, Required]
        Transform yawTransform;

        [SerializeField, Required]
        CharacterController controller;

        [SerializeField, Required]
        PlayerCameraController cameraController;

        [SerializeField, Required]
        PlayerSoundController soundController;

        [SerializeField]
        PlayerMovementConfig movementConfig;

        [SerializeField, InlineEditor]
        HeadBobData headBobConfig;

        [SerializeField, ReadOnly]
        PlayerMovementData movementData;

        CameraHandler cameraHandler;
        CrouchHandler crouchHandler;
        DirectionHandler directionHandler;
        JumpHandler jumpHandler;
        LandingHandler landingHandler;
        VelocityHandler velocityHandler;
        RunnningHandler runnningHandler;
        readonly PlayerCollisionData collisionData = new();
        IUpdateServices updateServices;

        protected override void Awake()
        {
            base.Awake();
            InitializeClasses();
            SetupStateMachine();
        }

        void InitializeClasses()
        {
            crouchHandler = new CrouchHandler(
                controller,
                yawTransform,
                movementConfig,
                collisionData,
                movementData
            );
            directionHandler = new DirectionHandler(
                controller,
                movementConfig,
                collisionData,
                movementData
            );
            jumpHandler = new JumpHandler(
                collisionData,
                movementData,
                movementConfig,
                soundController
            );
            landingHandler = new LandingHandler(
                yawTransform,
                collisionData,
                movementData,
                movementConfig
            );
            runnningHandler = new RunnningHandler(controller, movementConfig, movementData);
            cameraHandler = new CameraHandler(
                controller,
                movementConfig,
                collisionData,
                new HeadBobHandler(headBobConfig, movementData, movementConfig),
                movementData,
                cameraController,
                yawTransform,
                runnningHandler
            );
            velocityHandler = new VelocityHandler(
                controller,
                movementConfig,
                movementData,
                runnningHandler
            );
        }

        void SetupStateMachine()
        {
            var groundedState = new PlayerGroundedState(
                cameraHandler,
                crouchHandler,
                directionHandler,
                jumpHandler,
                landingHandler,
                velocityHandler,
                runnningHandler
            );
            var airborneState = new PlayerAirborneState(
                cameraHandler,
                crouchHandler,
                directionHandler,
                landingHandler,
                velocityHandler,
                runnningHandler
            );
            var underwaterState = new PlayerUnderwaterState(
                cameraHandler,
                crouchHandler,
                directionHandler,
                jumpHandler,
                landingHandler,
                velocityHandler,
                runnningHandler
            );
            var climbingState = new PlayerClimbingState(
                cameraHandler,
                crouchHandler,
                directionHandler,
                jumpHandler,
                landingHandler,
                velocityHandler,
                runnningHandler
            );

            At(groundedState, airborneState, !collisionData.OnGrounded);
            At(groundedState, climbingState, collisionData.OnClimbing);
            At(groundedState, underwaterState, collisionData.OnUnderwater);

            At(airborneState, groundedState, collisionData.OnGrounded);
            At(airborneState, underwaterState, collisionData.OnUnderwater);
            At(airborneState, climbingState, collisionData.OnClimbing);

            At(underwaterState, groundedState, collisionData.OnGrounded);
            At(underwaterState, airborneState, collisionData.OnAirborne);
            At(underwaterState, climbingState, collisionData.OnClimbing);

            At(climbingState, groundedState, collisionData.OnGrounded);
            At(climbingState, underwaterState, collisionData.OnUnderwater);
            At(climbingState, airborneState, collisionData.OnAirborne);

            stateMachine.SetState(groundedState);
        }

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        void OnDisable() => updateServices?.Unregister(this);

        public void ProcessUpdate(float deltaTime) => stateMachine.Update(deltaTime);
    }
}
