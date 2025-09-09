using System;
using GameToolkit.Runtime.Utils.Helpers;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerMovementConfig
    {
        [Header("Gravity Settings")]
        public float StickToGroundForce = 2.5f;
        public float GravityMultiplier = 2.5f;

        [Header("Gravity Settings")]
        public PID HorizontalPID = new(1.5f, 0f, 0.2f);
        public PID VerticalPID = new(5f, 0f, 0.5f);

        [Header("Walk Settings")]
        public float WalkSpeed = 5f;
        public float MoveBackwardsSpeedPercent = 0.5f;
        public float MoveSideSpeedPercent = 0.5f;

        [Header("Run Settings")]
        [Range(-1f, 1f)]
        public float CanRunThreshold = 0.8f;
        public float RunSpeed = 8f;
        public AnimationCurve RunTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Jump Settings")]
        public float JumpHeight = 1f;

        [Header("Crouch Settings")]
        [Range(0.2f, 0.9f)]
        public float CrouchSpeed = 2f;
        public float CrouchPercent = 0.6f;
        public float CrouchTransitionDuration = 1f;
        public AnimationCurve CrouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Landing Settings")]
        [Range(0.05f, 0.5f)]
        public float LowLandAmount = 0.1f;

        [Range(0.2f, 0.9f)]
        public float HighLandAmount = 0.6f;
        public float LandTimer = 0.5f;
        public float LandDuration = 1f;
        public AnimationCurve LandCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Smooth Settings")]
        public float SmoothInputSpeed = 10f;
        public float SmoothRotateSpeed = 10f;
        public float SmoothVelocitySpeed = 10f;
        public float SmoothFinalDirectionSpeed = 10f;
        public float SmoothHeadBobSpeed = 10f;
    }
}
