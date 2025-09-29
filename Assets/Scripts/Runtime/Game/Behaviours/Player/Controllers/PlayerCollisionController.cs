using Alchemy.Inspector;
using GameToolkit.Runtime.Game.Systems.Update;
using GameToolkit.Runtime.Utils.Tools.ServicesLocator;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class PlayerCollisionController : MonoBehaviour, IUpdatable
    {
        [SerializeField, Required]
        CharacterController controller;

        [SerializeField]
        PlayerCollisionConfig collisionConfig;

        [SerializeField, ReadOnly]
        PlayerCollisionData collisionData;

        IUpdateServices updateServices;
        GroundCheck groundCheck;
        ObstacleCheck obstacleCheck;
        RoofCheck roofCheck;
        CharacterPush characterPush;
        readonly PlayerMovementData movementData = new();

        void Awake()
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
                PreviouslyGrounded = true,
                FinalRayLength = collisionConfig.RayLength + controller.center.y
            };
        }

        void InitalizationClasses()
        {
            groundCheck = new GroundCheck(controller, collisionConfig, collisionData);
            obstacleCheck = new ObstacleCheck(
                controller,
                collisionConfig,
                movementData,
                collisionData
            );
            roofCheck = new RoofCheck(controller, collisionData);
            characterPush = new CharacterPush(controller, collisionConfig);
        }

        void OnEnable()
        {
            if (ServiceLocator.Global.TryGet(out updateServices))
                updateServices.Register(this);
        }

        void OnDisable() => updateServices?.Unregister(this);

        public void ProcessUpdate(float deltaTime)
        {
            groundCheck.CheckGround();
            obstacleCheck.CheckObstacle();
            roofCheck.CheckRoof();
            collisionData.PreviouslyGrounded = collisionData.OnGrounded;
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => characterPush.PushBody(hit);
    }
}
