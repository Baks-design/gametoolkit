using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerMovementConfig
    {
        [Header("Gravity Settings")]
        public float GravityMultiplier = 2.5f;
        public float StickToGroundForce = 1f;

        [Header("Walk Settings")]
        public float WalkSpeed = 3f;
        public float MoveBackwardsSpeedPercent = 0.5f;
        public float MoveSideSpeedPercent = 0.7f;

        [Header("Run Settings")]
        [Range(-1f, 1f)]
        public float CanRunThreshold = 0.7f;
        public float RunSpeed = 6f;
        public AnimationCurve RunTransitionCurve;

        [Header("Jump Settings")]
        public float JumpHeight = 6f;

        [Header("Crouch Settings")]
        public float CrouchSpeed = 1f;

        [Range(0.2f, 0.9f)]
        public float CrouchPercent = 0.6f;
        public float CrouchTransitionDuration = 0.5f;
        public AnimationCurve CrouchTransitionCurve;

        [Header("Landing Settings")]
        [Range(0.05f, 0.5f)]
        public float LowLandAmount = 0.1f;

        [Range(0.2f, 0.9f)]
        public float HighLandAmount = 0.4f;
        public float LandTimer = 0.5f;
        public float LandDuration = 0.5f;
        public AnimationCurve LandCurve;

        [Header("Smooth Settings")]
        public float SmoothInputSpeed = 10f;
        public float SmoothRotateSpeed = 10f;
        public float SmoothVelocitySpeed = 3f;
        public float SmoothFinalDirectionSpeed = 10f;
        public float SmoothHeadBobSpeed = 5f;

        [Header("Animation Settings")]
        public float MovementSmoothing = 0.1f;
    }
}
