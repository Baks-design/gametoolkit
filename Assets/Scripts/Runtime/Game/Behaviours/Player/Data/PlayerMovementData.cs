using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerMovementData
    {
        public Vector3 FinalMoveVelocity;
        public Vector3 FinalMoveDirection;
        public Vector3 FinalOffset;
        public Vector3 CurrentVelocity;
        public Vector3 SmoothFinalMoveDir;
        public Vector2 SmoothInputVector;
        public bool IsMoving;
        public bool IsWalking;
        public bool IsRunning;
        public bool IsCrouching;
        public bool IsClimbing;
        public bool IsSwimming;
        public bool IsJumping;
        public bool Resetted;
        public bool IsDuringRunAnimation;
        public bool IsDuringCrouchAnimation;
        public float InAirTimer;
        public float CurrentStateHeight;
        public float CurrentSpeed;
        public float VerticalVelocity;
        public float InitCamHeight;
    }
}
