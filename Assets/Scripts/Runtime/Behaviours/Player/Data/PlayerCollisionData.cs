using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerCollisionData
    {
        public RaycastHit CastHit;
        public Vector3 InitCenter;
        public Vector3 GroundedNormal;
        public Vector3 ObstructedNormal;
        public bool OnGrounded;
        public bool PreviouslyGrounded;
        public bool HasObstructed;
        public bool HasRoofed;
        public bool OnUnderwater;
        public bool OnAirborne;
        public bool OnClimbing;
        public float InitHeight;
        public float FinalRayLength;
        public float RoofRaySphereRadius;
    }
}
