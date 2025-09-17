using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [CreateAssetMenu(menuName = "Data/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget TransformTarget = TransformTarget.Rotation;
        public float Amplitude = 1f;
        public float Frequency = 0.5f;
    }
}
