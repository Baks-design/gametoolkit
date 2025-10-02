using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [CreateAssetMenu(menuName = "Data/Camera/HeadBobData")]
    public class HeadBobData : ScriptableObject
    {
        [Header("Curves")]
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;

        [Header("Amplitude")]
        public float xAmplitude = 0.05f;
        public float yAmplitude = 0.1f;

        [Header("Frequency")]
        public float xFrequency = 2f;

        [Header("Run Multipliers")]
        public float runAmplitudeMultiplier = 1.5f;
        public float runFrequencyMultiplier = 1.5f;

        [Header("Crouch Multipliers")]
        public float crouchAmplitudeMultiplier = 0.2f;
        public float crouchFrequencyMultiplier = 1f;

        public float MoveBackwardsFrequencyMultiplier { get; set; }
        public float MoveSideFrequencyMultiplier { get; set; }
    }
}
