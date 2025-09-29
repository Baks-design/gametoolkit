using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    [CreateAssetMenu(menuName = "Data/Camera/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget TransformTarget = TransformTarget.Rotation;
        public float Amplitude = 1f;
        public float Frequency = 0.5f;
    }
}
