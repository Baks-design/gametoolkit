using UnityEngine;

namespace GameToolkit.Runtime.Behaviours.Player
{
    [CreateAssetMenu(menuName = "Data/PerlinNoiseData")]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget TransformTarget;
        public float Amplitude;
        public float Frequency;
    }
}
