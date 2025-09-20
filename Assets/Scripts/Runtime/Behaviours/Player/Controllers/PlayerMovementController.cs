using Alchemy.Inspector;
using GameToolkit.Runtime.Systems.UpdateManagement;
using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public class PlayerMovementController : StatefulEntity, IUpdatable
    {
        [SerializeField]
        Transform yawTransform;

        [SerializeField]
        CharacterController controller;

        [SerializeField]
        PlayerCameraController cameraController;

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

        protected override void Awake()
        {
            base.Awake();
            InitializeClasses();
            SetupStateMachine();
        }

        void InitializeClasses()
        {
            crouchHandler = new CrouchHandler(
                this,
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
            jumpHandler = new JumpHandler(controller, collisionData, movementData, movementConfig);
            landingHandler = new LandingHandler(
                this,
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
                jumpHandler,
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

            At(groundedState, airborneState, !controller.isGrounded);
            At(groundedState, climbingState, collisionData.OnClimbing);
            At(groundedState, underwaterState, collisionData.OnUnderwater);

            At(airborneState, groundedState, controller.isGrounded);
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

        void OnEnable() => UpdateManager.Register(this);

        void OnDisable() => UpdateManager.Unregister(this);

        public void ProcessUpdate(float deltaTime) => stateMachine.Update(deltaTime);
    }
}
