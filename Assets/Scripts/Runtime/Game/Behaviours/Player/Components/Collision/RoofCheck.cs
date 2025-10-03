using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class RoofCheck
    {
        readonly CharacterController controller;
        readonly PlayerCollisionData collisionData;
        readonly PlayerCollisionConfig collisionConfig;

        public RoofCheck(
            CharacterController controller,
            PlayerCollisionData collisionData,
            PlayerCollisionConfig collisionConfig
        )
        {
            this.controller = controller;
            this.collisionData = collisionData;
            this.collisionConfig = collisionConfig;
        }

        public bool CheckRoof()
        {
            var hitRoof = Physics.SphereCast(
                controller.transform.position,
                collisionConfig.RoofRadius,
                Vector3.up,
                out _,
                collisionData.InitHeight,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore
            );

            collisionData.HasRoofed = hitRoof;
            return hitRoof;
        }
    }
}
