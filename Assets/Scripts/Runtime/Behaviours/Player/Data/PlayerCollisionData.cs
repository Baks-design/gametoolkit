using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public struct PlayerCollisionData
    {
        public Vector3 InitCenter;
        public float InitHeight;
        public bool OnGrounded;
        public RaycastHit CastHit;
        public Vector3 GroundedNormal;
        public bool PreviouslyGrounded;
        public bool HasObstructed;
        public float FinalRayLength;
        public Vector3 ObstructedNormal;
        public bool HasRoofed;
        public float RoofRaySphereRadius;
        public bool OnUnderwater;
        public bool OnAirborne;
        public bool OnClimbing;
    }
}
