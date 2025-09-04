using GameToolkit.Runtime.Utils.Tools.StatesMachine;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : StatefulEntity
    {
        [SerializeField]
        Transform targetRotation;

        [SerializeField]
        CharacterController controller;

        [SerializeField]
        PlayerCameraController cameraController;

        [SerializeField]
        HeadBobData headBobConfig;

        [SerializeField]
        PlayerMovementConfig movementConfig;

        PlayerCollisionData collisionData;
        PlayerMovement playerMovement;

        protected override void Awake()
        {
            base.Awake();
            InitializeClasses();
            SetupStateMachine();
        }

        void InitializeClasses() =>
            playerMovement = new PlayerMovement(
                controller,
                targetRotation,
                movementConfig,
                headBobConfig,
                cameraController
            );

        void SetupStateMachine()
        {
            var groundedState = new PlayerGroundedState(playerMovement);
            var airborneState = new PlayerAirborneState(playerMovement);
            var underwaterState = new PlayerUnderwaterState(playerMovement);
            var climbingState = new PlayerClimbingState(playerMovement);

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
    }
}
