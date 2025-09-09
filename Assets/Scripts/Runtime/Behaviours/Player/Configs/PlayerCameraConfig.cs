using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerCameraConfig
    {
        [Header("Look Settings")]
        public Vector2 Sensitivity = Vector2.zero;
        public Vector2 SmoothAmount = Vector2.zero;
        public Vector2 LookAngleMinMax = Vector2.zero;

        [Header("Breathing Settings")]
        public PerlinNoiseData Data;
        public bool X = true;
        public bool Y = true;
        public bool Z = true;

        [Header("Sway Settings")]
        public float SwayAmount = 0f;
        public float SwaySpeed = 0f;
        public float ReturnSpeed = 0f;
        public float ChangeDirectionMultiplier = 0f;
        public AnimationCurve SwayCurve = new();

        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)]
        public float ZoomFOV = 20f;
        public AnimationCurve ZoomCurve = new();
        public float ZoomTransitionDuration = 0f;

        [Range(60f, 100f)]
        public float RunFOV = 60f;
        public AnimationCurve RunCurve = new();
        public float RunTransitionDuration = 0f;
        public float RunReturnTransitionDuration = 0f;
    }
}
