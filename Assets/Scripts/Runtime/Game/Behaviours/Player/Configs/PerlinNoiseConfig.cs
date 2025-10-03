using UnityEngine;

namespace GameToolkit.Runtime.Game.Behaviours.Player
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }

    [CreateAssetMenu(menuName = "Data/Camera/PerlinNoiseData")]
    public class PerlinNoiseConfig : ScriptableObject
    {
        public TransformTarget TransformTarget = TransformTarget.Rotation;
        public float Amplitude = 1f;
        public float Frequency = 0.5f;
    }
}
