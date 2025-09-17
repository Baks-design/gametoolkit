using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
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

        public void CheckRoof()
        {
            var hitRoof = Physics.SphereCast(
                controller.transform.position,
                collisionData.RoofRaySphereRadius,
                Vector3.up,
                out var _,
                collisionData.InitHeight
            );

            collisionData.HasRoofed = hitRoof;
        }
    }
}
