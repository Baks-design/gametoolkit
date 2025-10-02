using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public class RoofCheck
    {
        readonly CharacterController controller;
        readonly PlayerCollisionData collisionData;

        public RoofCheck(CharacterController controller, PlayerCollisionData collisionData)
        {
            this.controller = controller;
            this.collisionData = collisionData;
        }

        public bool CheckRoof()
        {
            var hitRoof = Physics.SphereCast(
                controller.transform.position,
                controller.radius,
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
