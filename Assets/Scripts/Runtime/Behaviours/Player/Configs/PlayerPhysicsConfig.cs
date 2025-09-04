using System;
using Alchemy.Inspector;
using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [Flags]
    public enum LayerMaskFlags
    {
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,
    }

    [Serializable]
    public class PlayerCollisionConfig
    {
        [Header("Ground Settings")]
        [ReadOnly]
        public LayerMask GroundLayer = (int)LayerMaskFlags.Default;
        public float RaySphereRadius = 0.5f;
        public float RayLength = 0.5f;

        [Header("Obstacles Settings")]
        [ReadOnly]
        public LayerMask ObstacleLayers = (int)LayerMaskFlags.Default;
        public float RayObstacleSphereRadius = 0.5f;
        public float RayObstacleLength = 1f;
    }
}
