using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerMovementConfig
    {
        [Header("Locomotion Settings")]
        public float WalkSpeed = 5f;
        public float RunSpeed = 8f;
        public float JumpSpeed = 2.5f;
        public float MoveBackwardsSpeedPercent = 0.5f;
        public float MoveSideSpeedPercent = 0.5f;

        [Header("Run Settings")]
        [Range(-1f, 1f)]
        public float CanRunThreshold = 0.8f;
        public AnimationCurve RunTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Gravity Settings")]
        public float StickToGroundForce = 2.5f;
        public float GravityMultiplier = 2.5f;

        [Header("Smooth Settings")]
        public float SmoothInputSpeed = 10f;
        public float SmoothRotateSpeed = 10f;
        public float SmoothVelocitySpeed = 10f;
        public float SmoothFinalDirectionSpeed = 10f;
        public float SmoothHeadBobSpeed = 10f;
    }
}
