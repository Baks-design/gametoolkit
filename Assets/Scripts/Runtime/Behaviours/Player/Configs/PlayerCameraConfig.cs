using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Serializable]
    public class PlayerCameraConfig
    {
        [Header("Look Settings")]
        public Vector2 Sensitivity = new Vector2(40f, 40f);
        public Vector2 SmoothAmount = new Vector2(5f, 5f);
        public Vector2 LookAngleMinMax = new Vector2(-45f, 45f);

        [Header("Breathing Settings")]
        public bool X = true;
        public bool Y = false;
        public bool Z = false;

        [Header("Sway Settings")]
        public float SwayAmount = 1f;
        public float SwaySpeed = 1f;
        public float ReturnSpeed = 3f;
        public float ChangeDirectionMultiplier = 4f;
        public AnimationCurve SwayCurve;

        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)]
        public float ZoomFOV = 40f;
        public float ZoomTransitionDuration = 0.25f;
        public AnimationCurve ZoomCurve;

        [Range(60f, 100f)]
        public float RunFOV = 70f;
        public float RunTransitionDuration = 0.75f;
        public float RunReturnTransitionDuration = 0.5f;
        public AnimationCurve RunCurve;
    }
}
