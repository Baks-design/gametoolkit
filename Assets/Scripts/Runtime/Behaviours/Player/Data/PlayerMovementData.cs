using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerMovementData
    {
        public bool IsMoving;
        public bool IsWalking;
        public bool IsRunning;
        public bool IsCrouching;
        public float InAirTimer;
        public float CurrentStateHeight;
        public bool Resetted;
        public Vector3 FinalMoveVelocity;
        public Vector3 FinalMoveDirection;
        public Vector3 FinalOffset;
        public Vector3 CurrentVelocity;
    }
}
