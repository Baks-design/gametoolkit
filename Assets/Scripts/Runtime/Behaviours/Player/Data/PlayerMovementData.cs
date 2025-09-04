using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    public struct PlayerMovementData
    {
        public bool IsMoving;
        public bool IsWalking;
        public bool IsRunning;
        public float InAirTimer;
        public Vector3 FinalMoveVelocity;
        public Vector3 FinalMoveDirection;
    }
}
