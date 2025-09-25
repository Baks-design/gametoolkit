using System;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Flags]
    public enum LayerMaskFlags
    {
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Ground = 1 << 3,
        Water = 1 << 4,
        UI = 1 << 5,
        Player = 1 << 6
    }

    [Serializable]
    public class PlayerCollisionConfig
    {
        [Header("Ground Settings")]
        public LayerMask GroundLayer = (int)(LayerMaskFlags.Default | LayerMaskFlags.Ground);

        [Range(0.1f, 1f)]
        public float RayLength = 0.1f;

        [Range(0.1f, 1f)]
        public float RaySphereRadius = 0.2f;

        [Header("Obstacles Settings")]
        public LayerMask ObstacleLayers = (int)(LayerMaskFlags.Default | LayerMaskFlags.Ground);

        [Range(0.1f, 1f)]
        public float RayObstacleLength = 0.4f;

        [Range(0.1f, 1f)]
        public float RayObstacleSphereRadius = 0.2f;

        [Header("Push Settings")]
        public bool IsPushEnabled = true;
        public float PushPower = 2f;
        public float MaxPushForce = 10f;
        public bool UseForceInsteadOfVelocity = true;
        public ForceMode forceMode = ForceMode.Impulse;
    }
}
