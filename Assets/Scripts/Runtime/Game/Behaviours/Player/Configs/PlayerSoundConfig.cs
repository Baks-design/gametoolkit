using System;
using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [Serializable]
    public class PlayerSoundConfig
    {
        [Header("Footstep Sounds")]
        public float FootstepIntervalWalk = 0.5f;
        public float FootstepIntervalRun = 0.3f;
        public float FootstepVolume = 0.7f;

        [Header("Landing Sounds")]
        public float LandingVolume = 0.8f;

        [Header("Swimming Sounds")]
        public float SwimmingInterval = 0.4f;
        public float SwimmingVolume = 0.6f;

        [Header("Climbing Sounds")]
        public float ClimbingInterval = 0.6f;
        public float ClimbingVolume = 0.5f;
    }
}
