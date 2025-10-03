using System;
using Alchemy.Inspector;
using GameToolkit.Runtime.Application.Input;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerCollision : MonoBehaviour, IPlayerCollision
    {
        [SerializeField, Required]
        CharacterController controller;

        [SerializeField]
        PlayerCollisionConfig collisionConfig;

        [SerializeField, ReadOnly]
        PlayerCollisionData collisionData;

        [SerializeField, HideInInspector]
        PlayerMovementData movementData;

        GroundCheck groundCheck;
        ObstacleCheck obstacleCheck;
        RoofCheck roofCheck;
        CharacterPush characterPush;
        IMovementInput movementInput;

        void OnEnable() => ServiceLocator.Global.Get(out movementInput);

        void Start()
        {
            AdjustComponents();
            InitalizationClasses();
        }

        void AdjustComponents()
        {
            controller.center = new Vector3(0f, controller.height / 2f + controller.skinWidth, 0f);
            collisionData = new PlayerCollisionData
            {
                InitCenter = controller.center,
                InitHeight = controller.height,
                OnGrounded = true,
                OnAirborne = false,
                PreviouslyGrounded = true,
                FinalRayLength = collisionConfig.RayLength + controller.center.y
            };
        }

        void InitalizationClasses()
        {
            groundCheck = new GroundCheck(controller, collisionConfig, collisionData);
            obstacleCheck = new ObstacleCheck(
                movementInput,
                controller,
                collisionConfig,
                movementData,
                collisionData
            );
            roofCheck = new RoofCheck(controller, collisionData, collisionConfig);
            characterPush = new CharacterPush(controller, collisionData, collisionConfig);
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => characterPush.PushBody(hit);

        public void GroundCheckHandler() => groundCheck.CheckGround();

        public void ObstacleCheckHandler() => obstacleCheck.CheckObstacle();

        public bool RoofCheckHandler() => roofCheck.CheckRoof();
    }
}
